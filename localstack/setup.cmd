echo Starting localstack
docker-compose up -d
echo Setting config file
call aws --endpoint-url=http://localhost:4583 ssm put-parameter --name "/vote-results-dev/settings/electionsConfig" --type String --value "file://config.json" --overwrite --region "us-east-1"
echo Setting interval for sync
call aws --endpoint-url=http://localhost:4583 ssm put-parameter --name "/vote-results-dev/settings/intervalInSeconds" --type String --value "60" --overwrite --region "us-east-1"
pause