angular.module("umbraco").controller("FlexiForms.ViewController", function ($scope, $routeParams) {
	
	var vm = this;
	var flexiformsViewFolder = `${Umbraco.Sys.ServerVariables.umbracoSettings.appPluginsPath}/flexiForms/backoffice/responses`;
	var profileId = $routeParams.id

	vm.page = {};
	vm.page.name = `${profileId} Responses`;
	vm.page.navigation = [
		{
			"name": "Responses",
			"alias": "responseList",
			"icon": "icon-reply-arrow",
			"view": `${flexiformsViewFolder}/responses.html`,
			"active": true,
		},
		{
			"name": "Export",
			"alias": "exportResponses",
			"icon": "icon-download-alt",
			"view": `${flexiformsViewFolder}/exportResponses.html`,
			"active": false,
		}
	];

});