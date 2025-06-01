#!/bin/bash

set -e

echo "Setting up monitoring stack for the first time..."

# Function to force delete namespace
force_delete_namespace() {
    local namespace=$1
    echo "Force deleting namespace $namespace..."
    
    # Start kubectl proxy in background
    kubectl proxy --port=8001 &
    PROXY_PID=$!
    
    # Wait for proxy to start
    sleep 2
    
    # Remove finalizers using direct API call
    curl -k -H "Content-Type: application/json" -X PUT --data-binary '{"kind":"Namespace","apiVersion":"v1","metadata":{"name":"'$namespace'"},"spec":{"finalizers":[]}}' http://127.0.0.1:8001/api/v1/namespaces/$namespace/finalize
    
    # Kill proxy
    kill $PROXY_PID
    
    # Wait for namespace to be deleted
    echo "Waiting for namespace to be deleted..."
    timeout=60
    while [ $timeout -gt 0 ]; do
        if ! kubectl get namespace $namespace &>/dev/null; then
            echo "Namespace deleted successfully"
            return 0
        fi
        echo "Still waiting... ($timeout seconds remaining)"
        sleep 2
        timeout=$((timeout - 2))
    done
    
    echo "Failed to delete namespace after timeout"
    return 1
}

# Check if namespace exists and is terminating
if kubectl get namespace monitoring-new &>/dev/null; then
    echo "Namespace exists, checking status..."
    if kubectl get namespace monitoring-new -o jsonpath='{.status.phase}' | grep -q "Terminating"; then
        echo "Namespace is terminating. Force cleaning up..."
        force_delete_namespace "monitoring-new"
    else
        echo "Deleting existing namespace..."
        kubectl delete namespace monitoring-new
        echo "Waiting for namespace to be deleted..."
        timeout=60
        while [ $timeout -gt 0 ]; do
            if ! kubectl get namespace monitoring-new &>/dev/null; then
                echo "Namespace deleted successfully"
                break
            fi
            echo "Still waiting... ($timeout seconds remaining)"
            sleep 2
            timeout=$((timeout - 2))
        done
    fi
fi

echo "Creating monitoring namespace..."
kubectl create namespace monitoring-new
echo "Namespace created successfully"

echo "Creating Prometheus ConfigMap..."
kubectl apply -f ../../Infra/monitoring/prometheus-config.yaml
echo "Prometheus ConfigMap created"

echo "Creating Prometheus Deployment and Service..."
kubectl apply -f ../../Infra/monitoring/prometheus-deployment.yaml
echo "Prometheus Deployment and Service created"

echo "Creating Loki Deployment and Service..."
kubectl apply -f ../../Infra/monitoring/loki-deployment.yaml
echo "Loki Deployment and Service created"

echo "Creating Grafana Deployment and Service..."
kubectl apply -f ../../Infra/monitoring/grafana-deployment.yaml
echo "Grafana Deployment and Service created"

echo "Waiting for pods to be ready..."
echo "Checking pod status..."
kubectl get pods -n monitoring-new

echo "Waiting for Grafana pod..."
kubectl wait --for=condition=ready pod -l "app=grafana" -n monitoring-new --timeout=300s || {
    echo "Grafana pod failed to become ready. Checking pod status..."
    kubectl get pods -n monitoring-new -l "app=grafana"
    kubectl describe pods -n monitoring-new -l "app=grafana"
    exit 1
}
echo "Grafana pod is ready"

echo "Waiting for Prometheus pod..."
kubectl wait --for=condition=ready pod -l "app=prometheus" -n monitoring-new --timeout=300s || {
    echo "Prometheus pod failed to become ready. Checking pod status..."
    kubectl get pods -n monitoring-new -l "app=prometheus"
    kubectl describe pods -n monitoring-new -l "app=prometheus"
    exit 1
}
echo "Prometheus pod is ready"

echo "Waiting for Loki pod..."
kubectl wait --for=condition=ready pod -l "app=loki" -n monitoring-new --timeout=300s || {
    echo "Loki pod failed to become ready. Checking pod status..."
    kubectl get pods -n monitoring-new -l "app=loki"
    kubectl describe pods -n monitoring-new -l "app=loki"
    exit 1
}
echo "Loki pod is ready"

echo "Monitoring setup complete!"
echo -e "\nTo access the monitoring tools, run these commands in separate terminals:"
echo "Grafana:            kubectl port-forward -n monitoring-new svc/grafana 3000:3000"
echo "Prometheus:         kubectl port-forward -n monitoring-new svc/prometheus 9090:9090"
echo "Loki:              kubectl port-forward -n monitoring-new svc/loki 3100:3100" 