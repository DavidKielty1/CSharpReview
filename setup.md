Make sure we are portforwarding to hit endpoints

kubectl port-forward -n userdistributed svc/userdistributed-api 8080:80
