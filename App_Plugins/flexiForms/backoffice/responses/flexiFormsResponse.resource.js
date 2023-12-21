angular.module('umbraco.resources').factory('FlexiFormsResponsesResource', function($q, $http, umbRequestHelper) {

  const baseUrl = "FlexiForms/Responses";

  return {

    GetResponses: (formIdentifier) => {
      return umbRequestHelper.resourcePromise(
        $http.get(`${baseUrl}/GetResponses?formIdentifier=${formIdentifier}`),
        `Failed to get ${formIdentifier} responses`
      );
    },
		
		DeleteResponse: (responseId) => {
      return umbRequestHelper.resourcePromise(
        $http.delete(`${baseUrl}/DeleteResponse?responseId=${responseId}`),
        `Failed to delete response with ID ${responseId}`
      );
    },
		
		DeleteAllByIdentifier: (formIdentifier) => {
      return umbRequestHelper.resourcePromise(
        $http.delete(`${baseUrl}/DeleteAllByIdentifier?formIdentifier=${formIdentifier}`),
        `Failed to delete responses with form alias ${formIdentifier}`
      );
    },
		
		ExportFormResponses: (formIdentifier) => {
      return umbRequestHelper.downloadFile(`${baseUrl}/ExportFormResponses?formIdentifier=${formIdentifier}`);
    },
		
		GetAllFormsWithReplies: () => {
			return umbRequestHelper.resourcePromise(
        $http.get(`${baseUrl}/GetAllFormsWithReplies`),
        `Failed to get forms with replies`
      );
		}

  }

}); 