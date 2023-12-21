angular.module("umbraco").controller("FlexiForms.ResponseViewController", function ($scope, $location, $routeParams, appState, FlexiFormsResponsesResource, editorService, overlayService, localizationService) {
	
	var vm = this;
	var flexiformsViewFolder = `${Umbraco.Sys.ServerVariables.umbracoSettings.appPluginsPath}/flexiForms/backoffice/responses`;
	var profileId = $routeParams.id
	var currentSection = appState.getSectionState("currentSection");
	
	
	vm.loading = false;
	vm.responses = [];
	vm.filter = { searchTerm: "" };
	vm.pageName = `${profileId} Responses`;
	
	
	function deleteFormResponse(response) {

		const dialog = {
			view: `${flexiformsViewFolder}/deleteOverlay.html`,
			submitButtonLabelKey: "contentTypeEditor_yesDelete",
			submitButtonStyle:"danger",
			response: response,
			submit: function (model) {
				FlexiFormsResponsesResource.DeleteResponse(response.id)
					.then(responseId => {
						vm.responses = vm.responses.filter((response) => response.id != responseId);
						overlayService.close();
						editorService.close();
					})
					.catch(ex => {
						overlayService.close();
						editorService.close();
					});
			},
			close: function () {
				overlayService.close();
			}
		};

		const keys = [
			"general_delete",
			"defaultdialogs_confirmdelete"
		];

		localizationService.localizeMany(keys).then(values => {
			dialog.title = values[0];
			dialog.content = values[1];
			overlayService.open(dialog);
		});

	}

	
	
	vm.openResponse = function (response) {
		const responseDetails = {
			title: "Response",
			size: "small",
			response: response,
			view: `${flexiformsViewFolder}/response.html`,
			close: function() {
				editorService.close();
			},
			delete: function() {
				deleteFormResponse(response);
			}
		};

		editorService.open(responseDetails);
	};
	

	function init() {
		vm.loading = true;
		
		FlexiFormsResponsesResource.GetResponses(profileId)
		.then(data => {
			vm.responses = data;
		});
			
		vm.loading = false;
		
	}
	
	
	init();

});