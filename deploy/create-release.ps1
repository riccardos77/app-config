helm package ./chart/riccardos77-appconfig --dependency-update --destination ./charts-temp
helm repo index --merge ./charts/index.yaml ./charts-temp
Move-Item ./charts-temp/*.* ./charts -Force
Remove-Item ./charts-temp
