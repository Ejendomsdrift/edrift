var ToastService = function (ngToast, $filter) {
        var self = this;
        self.saveInProgressMessage = null;

        function dismissMessage(message) {
            if (message) {
                ngToast.dismiss(message);
            }
        }

        self.showToastMessage = function (className, content, dismissOnTimeout) {
            if (typeof dismissOnTimeout === "undefined") {
                dismissOnTimeout = true;
            }

            return ngToast.create({
                className: className,
                content: content,
                timeout: 3000,
                dismissButton: true,
                compileContent: true,
                dismissOnTimeout: dismissOnTimeout
            });
        }

        self.showToastInfoMessage = function (resourceKey) {
            return self.showToastMessage('info', '<div>{{"' + resourceKey + '" | translate}}</div>', false);
        }

        self.showToastSuccessMessage = function (resourceKey) {
            dismissMessage(self.saveInProgressMessage);
            return self.showToastMessage('success', '<div>{{"' + resourceKey + '" | translate}}</div>');
        }

        self.showToastDangerMessage = function (resourceKey) {
            dismissMessage(self.saveInProgressMessage);
            return self.showToastMessage('danger', '<div>{{"' + resourceKey + '" | translate}}</div>');
        }

        self.showToastInProgressMessage = function (resourceKey) {
            if (!resourceKey) {
                resourceKey = 'Save in progress';
            }

            self.saveInProgressMessage = self.showToastInfoMessage(resourceKey);
            return self.saveInProgressMessage;
        }

        self.showToastSaveSuccessMessage = function (message) {
            var msg = message || 'Save success';
            self.showToastSuccessMessage($filter('translate')(msg));
        }

        self.showToastFailMessage = function (message) {
            var msg = message || 'Save failed';
            self.showToastDangerMessage($filter('translate')(msg));
        }
}

angular.module('boligdrift').service('toastService', ['ngToast', '$filter', ToastService]);