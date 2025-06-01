#!/bin/bash

set -e

IMAGE_NAME="userdistributed-api:latest"
NAMESPACE="userdistributed"
PORT_LOCAL=8080
PORT_REMOTE=80

echo "1. Building Docker image..."
docker build -t $IMAGE_NAME -f ../../Dockerfile ../../

echo "2. Applying Kubernetes manifests..."
kubectl apply -f ../../Infra/k8s/namespace.yaml
kubectl apply -f ../../Infra/k8s/sqlserver.yaml
kubectl apply -f ../../Infra/k8s/redis.yaml
kubectl apply -f ../../Infra/k8s/api.yaml
kubectl apply -f ../../Infra/k8s/hpa.yaml

echo "3. Checking monitoring setup..."
if ! kubectl get namespace monitoring-new &>/dev/null || \
   ! kubectl get deployment prometheus -n monitoring-new &>/dev/null || \
   ! kubectl get deployment grafana -n monitoring-new &>/dev/null; then
    echo "Monitoring stack not found. To set up monitoring, run:"
    echo "./docs/bash/setup-monitoring-initial.sh"
    echo "This only needs to be done once."
else
    echo "Monitoring stack is ready!"
fi

echo "4. Restarting API deployment..."
kubectl rollout restart deployment/userdistributed-api -n $NAMESPACE

echo "5. Waiting for API pods to be ready..."
kubectl rollout status deployment/userdistributed-api -n $NAMESPACE --timeout=300s

echo "Deployment complete!"
echo -e "\nTo access the API, run:"
echo "kubectl port-forward -n $NAMESPACE svc/userdistributed-api 8080:80"
echo -e "\nTo access monitoring tools, run these in separate terminals:"
echo "Grafana:            kubectl port-forward -n monitoring-new svc/grafana 3000:3000"
echo "Prometheus:         kubectl port-forward -n monitoring-new svc/prometheus 9090:9090"