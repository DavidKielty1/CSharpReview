#!/bin/bash

NAMESPACE="userdistributed"
APP_LABEL="userdistributed-api"

# Get the first pod name matching the app label
POD=$(kubectl get pods -n $NAMESPACE -o jsonpath="{.items[?(@.metadata.labels.app=='$APP_LABEL')].metadata.name}" | awk '{print $1}')

if [ -z "$POD" ]; then
  echo "No pod found for app=$APP_LABEL in namespace $NAMESPACE"
  exit 1
fi

echo "Tailing logs for pod: $POD"
kubectl logs -f $POD -n $NAMESPACE