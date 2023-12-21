angular.module("umbraco").controller("FlexiForms.ExportResponsesController", function ($scope, FlexiFormsResponsesResource, $routeParams) {
	
	var vm = this;
	var profileId = $routeParams.id

	vm.loading = false;

	vm.exportButton = {
		label: "Export",
		state: "init",
		action: exportResponses
	};
	
	function exportResponses() {
		vm.exportButton.state = "busy";
		FlexiFormsResponsesResource.ExportFormResponses(profileId)
			.then(res => {
				vm.exportButton.state = "success";
			})
			.catch(ex => {
				vm.exportButton.state = "error";
			})
	}
	
});