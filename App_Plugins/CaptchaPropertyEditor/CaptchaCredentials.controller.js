
angular.module('umbraco').controller('CaptchaCredentialsController', function ($scope, validationMessageService) {
	
	
	$scope.config = {
		items: [],
		selected: null
	};
	
	
	function getCredentialHash(captchaCredential) {
		return JSON.stringify(captchaCredential);
	}
	
	
	function init() {
		const { captchaCredentials } = $scope.model.config;
		$scope.config.items = captchaCredentials.map(captchaCredential => ({ ...captchaCredential, value: getCredentialHash(captchaCredential) }));
		var found = captchaCredentials.find(captchaCredential => 
			captchaCredential.alias == $scope.model.value?.alias &&
			captchaCredential.clientSecret == $scope.model.value?.clientSecret &&
			captchaCredential.sitekey == $scope.model.value?.sitekey
		);
		
		$scope.config.selected = found 
			? getCredentialHash(found)
			: null;
	}
	
	
	$scope.updateCaptchaProvider = function() {
		var found = $scope.config.items.find(viewCaptcha => viewCaptcha.value == $scope.config.selected);	
		if (found) {
			const {value: _, ...captchaCredential} = found;
			$scope.model.value = captchaCredential;
		}
		else {
			$scope.model.value = null;
		}
	}
	
	validationMessageService.getMandatoryMessage($scope.model.validation).then(function (value) {
		$scope.mandatoryMessage = value;
	});


	init();
  
	
});