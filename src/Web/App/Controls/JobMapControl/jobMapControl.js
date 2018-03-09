(function () {
    var jobMapControl = function (janitorService) {
        var self = {
            restrict: 'E',
            templateUrl: '/App/Controls/JobMapControl/jobMapControl.html',
            scope: {
                jobs: '@'
            }
        }

        self.link = function (scope, element) {
            var defaultMapOptions = {
                center: new google.maps.LatLng(55.676098, 12.568337),
                zoom: 8, 
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };

            function initMap() {
                var map = new google.maps.Map(element[0].querySelector(".map-container"), defaultMapOptions);

                initListeners(map);
            }

            function initListeners(map) {
                var listener = scope.$watch('jobs', function (value, oldvalue) {
                    google.maps.event.trigger(map, 'resize');
                    if (value != '') {
                        var jobs = JSON.parse(value);
                        if (angular.isArray(jobs)) {
                            var geocoder = new google.maps.Geocoder();
                            var bounds = new google.maps.LatLngBounds();
                            jobs.forEach(function (job) {
                                geocodeAddress(map, geocoder, job, bounds);
                            });
                            setTimeout(function () {
                                map.fitBounds(bounds);
                            }, 1000);
                        }
                    }
                });

                scope.$on('$destroy', listener);
            }

            function geocodeAddress(map, geocoder, job, bounds) {
                if (job.address) {
                    geocoder.geocode({ address: job.address }, function (results, status) {
                        if (status === google.maps.GeocoderStatus.OK) {
                            setMarker(map, results[0].geometry.location, job, bounds);
                        }
                    });
                }
            }

            function setMarker(map, location, job, bounds) {
                var marker = new google.maps.Marker({
                    map: map,
                    position: location
                });
                var today = new Date();
                var jobDate = new Date(job.date);
                var isOverdue = jobDate < today;
                marker.setIcon(isOverdue
                    ? 'http://maps.google.com/mapfiles/ms/icons/red-dot.png'
                    : 'http://maps.google.com/mapfiles/ms/icons/green-dot.png');

                var content = "<div>";
                content += "<div class='map__title'>" + job.title + "</div>";
                content += isOverdue ? "<span class='map__date _font-red'>" : "<span>";
                content += janitorService.getFormatedDateString(job.date);
                if (job.jobType === JobType.Tenant || job.jobType === JobType.Other) {
                    content += " " + janitorService.getFormatedTimeString(job.date);
                    if (job.estimate) {
                        content += " - " + janitorService.getEstimateTimeString(job.date, job.estimate);
                    }
                }
                content += "</span>";
                content += "<div class='map__address'>" + job.address + "</span>";
                content += "</div>";

                var info = new google.maps.InfoWindow({
                    content: content
                });

                markerListener(marker, info);
                bounds.extend(location);
            }

            function markerListener(marker, info) {
                marker.addListener('click', function () {
                    info.open(marker.get('map'), marker);
                });
            }

            initMap();
        }

        return self;
    }

    angular.module('boligdrift').directive('jobMapControl', ['janitorService', jobMapControl]);
})();