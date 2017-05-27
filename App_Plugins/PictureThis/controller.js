(function () {
    'use strict';

    function ctrl(dialogService, pictureThisService, notificationsService) {

        var vm = this;
        
        pictureThisService.get()
            .then(function (resp) {
                vm.image = resp.image.startsWith('/media') ? resp.image : '/umbraco/' + resp.image;
            });

        function setImage() {
            dialogService.mediaPicker({
                callback: function (resp) {
                    vm.image = resp.image;
                    vm.showButton = true;
                } 
            });
        }

        function save() {
            pictureThisService.save(vm.image)
                .then(function (resp) {
                    if (resp.status && resp.status === 200) {
                        notificationsService.success('Success!', 'Background image updated');
                    } else {
                        notificationsService.error('Error!', 'Something went wrong');
                    }
                });
        }

        angular.extend(vm, {            
            setImage: setImage,
            save: save
        });
    }

    angular.module('umbraco').controller('pictureThisCtrl', ['dialogService', 'pictureThisService', 'notificationsService', ctrl]);

}());