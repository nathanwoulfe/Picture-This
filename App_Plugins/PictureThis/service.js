(function () {
    'use strict';

    function service($http, umbRequestHelper) {
        
        var base = '/umbraco/backoffice/picturethis/config/';

        function request(method, url, data) {
            return umbRequestHelper.resourcePromise(
                method === 'POST' ? $http.post(url, data) : $http.get(url), 'Something broke'
            );
        }

        function save(image) {
            return request('POST', base + 'post', { image: image });
        }

        function get() {
            return request('GET', base + 'get');
        }

        return {
            save: save,
            get: get
        }
    }

    angular.module('umbraco').service('pictureThisService', ['$http', 'umbRequestHelper', service]);

}());