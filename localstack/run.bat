docker-compose up -d
aws --endpoint-url=http://localhost:4583 ssm put-parameter --name "/vote-results-dev/settings/intervalInSeconds" --type String --value "60" --overwrite --region "us-east-1"
aws --endpoint-url=http://localhost:4583 ssm put-parameter --name "/vote-results-dev/settings/electionsConfig" --type String --value "file://config.json" --overwrite --region "us-east-1"
pause