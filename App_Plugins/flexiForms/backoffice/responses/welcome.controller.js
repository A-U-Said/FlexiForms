angular.module("umbraco").controller("FlexiForms.WelcomeController", function ($scope, $location, appState, FlexiFormsResponsesResource) {
	
	var vm = this;
	var currentSection = appState.getSectionState("currentSection");
	
	vm.loading = false;

	vm.page = {
		title: "FlexiForms",
		description: "A straightforward and flexible forms solution"
	};

	vm.formsWithResponses = [];

	vm.goToResponses = (identifier) => {
		$location.path(`/${currentSection}/responses/view/${identifier}`);
	}
	
	function init() {
		vm.loading = true;
		FlexiFormsResponsesResource.GetAllFormsWithReplies()
			.then(res => {
				vm.formsWithResponses = res;
				vm.loading = false;
			})
			.catch(ex => {
				vm.loading = false;
			})
	}
	
	init();

});