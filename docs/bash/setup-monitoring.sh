#!/bin/bash

set -e

echo "1. Installing Prometheus stack with custom Grafana configuration..."
helm upgrade --install prometheus prometheus-community/kube-prometheus-stack \
    --namespace monitoring --create-namespace \
    --set prometheus.prometheusSpec.maximumStartupDurationSeconds=600 \
    --set grafana.adminPassword=Demonadmo123! \
    --set grafana.persistence.enabled=true \
    --set grafana.persistence.size=10Gi \
    --set grafana.service.type=ClusterIP \
    --set grafana.service.port=80 \
    --set grafana.service.targetPort=3000 \
    --set grafana.env.GF_AUTH_ANONYMOUS_ENABLED=true \
    --set grafana.env.GF_AUTH_ANONYMOUS_ORG_ROLE=Viewer \
    --set grafana.env.GF_SECURITY_ALLOW_EMBEDDING=true \
    --set-file 'grafana\\.datasources\\.yaml=../../Infra/monitoring/datasources.yaml' \
    --set-file 'grafana\\.dashboards\\.default\\.api-scaling\\.json=../../Infra/monitoring/api-dashboard.json'

echo "Monitoring setup complete!"
echo -e "\nTo access the monitoring tools, run these commands in separate terminals:"
echo "Grafana:            kubectl port-forward -n monitoring svc/prometheus-grafana 3000:80"
echo "Loki:               kubectl port-forward -n monitoring svc/prometheus-loki 3100:3100"
echo "Prometheus:         kubectl port-forward -n monitoring svc/prometheus-kube-prometheus-prometheus 9090:9090" 