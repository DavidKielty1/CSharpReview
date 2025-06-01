#!/bin/bash

set -e

echo "Checking if services are ready..."

# Function to wait for a service
wait_for_service() {
    local service=$1
    local namespace=$2
    local timeout=300
    local elapsed=0
    local interval=5

    echo "Waiting for $service to be ready in namespace $namespace..."
    while [ $elapsed -lt $timeout ]; do
        if kubectl get svc $service -n $namespace &>/dev/null; then
            echo "$service is ready!"
            return 0
        fi
        echo "Still waiting for $service... ($elapsed/$timeout seconds)"
        sleep $interval
        elapsed=$((elapsed + interval))
    done
    echo "Timeout waiting for $service"
    return 1
}

# Wait for all services
wait_for_service "prometheus" "monitoring-new" || exit 1
wait_for_service "grafana" "monitoring-new" || exit 1
wait_for_service "loki" "monitoring-new" || exit 1
wait_for_service "userdistributed-api" "userdistributed" || exit 1

echo "Starting port forwarding..."

# Start port forwarding in background
echo "Forwarding Grafana to http://localhost:3000"
kubectl port-forward -n monitoring-new svc/grafana 3000:3000 &
GRAFANA_PID=$!

echo "Forwarding Prometheus to http://localhost:9090"
kubectl port-forward -n monitoring-new svc/prometheus 9090:9090 &
PROMETHEUS_PID=$!

echo "Forwarding Loki to http://localhost:3100"
kubectl port-forward -n monitoring-new svc/loki 3100:3100 &
LOKI_PID=$!

echo "Forwarding API to http://localhost:8080"
kubectl port-forward -n userdistributed svc/userdistributed-api 8080:80 &
API_PID=$!

# Function to handle script termination
cleanup() {
    echo "Stopping port forwarding..."
    kill $GRAFANA_PID $PROMETHEUS_PID $LOKI_PID $API_PID 2>/dev/null
    exit 0
}

# Set up trap for cleanup
trap cleanup SIGINT SIGTERM

echo "Port forwarding is active. Press Ctrl+C to stop."
echo "Grafana:     http://localhost:3000 (admin/Demonadmo123!)"
echo "Prometheus:  http://localhost:9090"
echo "Loki:        http://localhost:3100"
echo "API:         http://localhost:8080"

# Keep script running
wait 