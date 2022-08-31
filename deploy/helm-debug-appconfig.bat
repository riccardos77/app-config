helm dependency update ./chart/riccardos77-appconfig
helm template --debug appconfig-only ./chart/riccardos77-appconfig > ./chart/riccardos77-appconfig/test.yaml
