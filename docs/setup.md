Make sure we are portforwarding to hit endpoints

## Port forward

kubectl port-forward -n userdistributed svc/userdistributed-api 8080:80

## Check Kubs

kubectl get pods -n userdistributed

kubectl logs -f <userdistributed-api-pods> -n userdistributed

## Load testing kub

hey -n 100 -c 10 http://localhost:8080/api/userparallel/metrics
beaverhall
