@REM docker tag riccardos77-appconfig riccardos77/app-config:latest
docker push riccardos77/app-config:%1
docker tag riccardos77/app-config:%1 riccardos77/app-config:latest
docker push riccardos77/app-config:latest
