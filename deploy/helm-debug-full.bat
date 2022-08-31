helm dependency update ./chart/riccardos77-appconfig
helm dependency update ./chart/test-parent-chart
helm template --debug test-parent-chart ./chart/test-parent-chart > ./chart/test-parent-chart/test.yaml
