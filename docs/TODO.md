- Test metrics endpoint and see if grafana/loki show kubernetes pod scaling

- Test redis/MSSQL queries and mutations

--GitBash
deploy
prtforward

--WSL
kubectl port-forward -n userdistributed svc/userdistributed-api 8080:80
hey -z 1m -c 20 http://localhost:8080/api/userparallel/metrics
