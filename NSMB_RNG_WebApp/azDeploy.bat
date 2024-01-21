@ECHO off

echo "Running infrastructure deployment"
call az deployment sub create --template-file NSMB_RNG_WebApp/main.bicep -l eastus -o none
echo "Running software deployment"
call az webapp deployment source config-zip --src pub.zip -n NSMB-RNG -g NSMB_RNG
