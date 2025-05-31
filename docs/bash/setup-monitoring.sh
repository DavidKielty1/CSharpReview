#!/bin/bash

set -e

echo "1. Installing Prometheus stack..."
helm upgrade --install prometheus prometheus-community/kube-prometheus-stack \
    --namespace monitoring --create-namespace \
    --set prometheus.prometheusSpec.maximumStartupDurationSeconds=600

echo "2. Installing Grafana..."
helm upgrade --install grafana grafana/grafana \
    --namespace monitoring \
    --values ../../Infra/monitoring/grafana-helm-values.yaml \
    --set-file 'datasources\\.yaml=../../Infra/monitoring/datasources.yaml'

echo "Monitoring setup complete!"
echo -e "\nTo access the monitoring tools, run these commands in separate terminals:"
echo "Grafana:            kubectl port-forward -n monitoring svc/grafana 3000:80"
echo "Loki:               kubectl port-forward -n monitoring svc/loki-stack 3100:3100"
echo "Prometheus:         kubectl port-forward -n monitoring svc/prometheus-kube-prometheus-prometheus 9090:9090" 