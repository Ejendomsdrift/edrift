(function () {
    var fileDownloader = function (uploadDataProvider) {
        var self = {
            restrict: 'A',
            scope: { }
        };

        self.link = function (scope, element, attrs) {
            element.bind("click", function (event) {
                event.preventDefault();
                event.stopPropagation();
                var fileName = element[0].getAttribute('href');
                var originalFileName = getOriginalFileName(element[0]);
                var downloadLink = uploadDataProvider.getDownloadLink(fileName, originalFileName);
                var link = getATag(downloadLink);

                link.click();
                document.body.removeChild(link);
            });
        };

        function getOriginalFileName(element) {
            var fileName = element.getAttribute('download');
            if (!fileName) {
                fileName = element.textContent.trim();
            } 

            return fileName;
        }

        function getATag(downloadLink) {
            var link = document.createElement("a");
            link.setAttribute("href", downloadLink);
            link.style = "visibility:hidden";
            document.body.appendChild(link);
            return link;
        }

        return self;
    }

    angular.module('boligdrift').directive('fileDownloader', ['uploadDataProvider', fileDownloader]);
})();
