#!/bin/bash

set -e

echo "Checking monitoring setup..."
if ! kubectl get namespace monitoring-new &>/dev/null; then
    echo "Monitoring namespace not found. Please run setup-monitoring-initial.sh first."
    exit 1
fi

echo "Checking Prometheus..."
if ! kubectl get deployment prometheus -n monitoring-new &>/dev/null; then
    echo "Prometheus not found. Please run setup-monitoring-initial.sh first."
    exit 1
fi

echo "Checking Grafana..."
if ! kubectl get deployment grafana -n monitoring-new &>/dev/null; then
    echo "Grafana not found. Please run setup-monitoring-initial.sh first."
    exit 1
fi

echo "Monitoring stack is ready!"
echo -e "\nTo access the monitoring tools, run these commands in separate terminals:"
echo "Grafana:            kubectl port-forward -n monitoring-new svc/grafana 3000:3000"
echo "Prometheus:         kubectl port-forward -n monitoring-new svc/prometheus 9090:9090" 