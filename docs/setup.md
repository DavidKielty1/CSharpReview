Make sure we are portforwarding to hit endpoints

# Setup

./deploy.sh

## Port forward

kubectl port-forward -n userdistributed svc/userdistributed-api 8080:80
kubectl port-forward -n monitoring svc/grafana 3000:80
kubectl port-forward -n monitoring svc/loki-stack 3100:3100

## Check Kubs

kubectl get pods -n userdistributed
kubectl get svc -A
kubectl get pods -n monitoring | grep grafana

kubectl logs -f <userdistributed-api-pods> -n userdistributed

## Load testing kub

hey -n 100 -c 10 http://localhost:8080/api/userparallel/metrics
beaverhall
