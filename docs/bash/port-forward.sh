#!/bin/bash

set -e

echo "Starting port-forwarding for all services..."
echo "Press Ctrl+C to stop all port-forwards"

# Function to handle cleanup on script exit
cleanup() {
    echo -e "\nStopping all port-forwards..."
    kill $(jobs -p) 2>/dev/null
    exit 0
}

# Set up trap for cleanup
trap cleanup SIGINT SIGTERM

# Start all port-forwards in background
echo "API: http://localhost:8080"
kubectl port-forward -n userdistributed svc/userdistributed-api 8080:80 &
echo "Grafana: http://localhost:3000"
kubectl port-forward -n monitoring svc/prometheus-grafana 3000:80 &
echo "Loki: http://localhost:3100"
kubectl port-forward -n monitoring svc/prometheus-loki 3100:3100 &
echo "Prometheus: http://localhost:9090"
kubectl port-forward -n monitoring svc/prometheus-kube-prometheus-prometheus 9090:9090 &

# Give the port-forwards a moment to start
sleep 2

# Open Grafana in the default browser
echo -e "\nOpening Grafana in your default browser..."
start http://localhost:3000

# Wait for all background processes
wait 