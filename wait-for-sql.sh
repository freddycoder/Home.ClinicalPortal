#!/bin/sh
# wait-for-postgres.sh

set -e
  
host="$1"
shift
  
sleep 20
  
>&2 echo "Fhir is now awaking"
exec "$@"

