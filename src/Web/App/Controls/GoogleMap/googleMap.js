(function () {
    var googleMap = function () {
        var self = {
            restrict: 'E',
            templateUrl: '/App/Controls/GoogleMap/googleMap.html',
            scope: {
                address: '@',
                onMapClicked: '&',
                isMarkerDisabled: '=?',
                isJanitorPlatform: '=?'
            }
        }

        self.link = function (scope, element) {
            var prewMarker;
            var defaultMapOptions = {
                center: new google.maps.LatLng(55.676098, 12.568337),
                zoom: 8,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };

            function initMap() {
                var map = new google.maps.Map(element[0].querySelector(".map-container"), defaultMapOptions);

                map.addListener('click', function (e) {
                    if (!scope.isMarkerDisabled) {
                        placeMarkerAndPanTo(e.latLng, map);
                    }
                });

                initListeners(map);
            }

            function initListeners(map) {
                var listener = scope.$watch('address', function (value, oldvalue) {
                    google.maps.event.trigger(map, 'resize');
                    geocodeAddress(map, value);
                });

                scope.$on('$destroy', listener);
            }

            function geocodeAddress(map, address) {
                var geocoder = new google.maps.Geocoder();
                var settings = {};
                if (address) {
                    settings = { address: address };
                }
                
                geocoder.geocode(settings, function (results, status) {
                    if (status === google.maps.GeocoderStatus.OK) {
                        if (address) {
                            scope.isJanitorPlatform ? setMarkerForJanitorPlatform(results[0].geometry.location) : setMarkerForManagementPlatform(map, results[0].geometry.location);
                        }
                    } else {
                        clearPrewMarker(prewMarker);
                        map.setCenter(defaultMapOptions.center);
                        map.setZoom(defaultMapOptions.zoom);
                    }
                });
            }

            function setMarkerForManagementPlatform(map, location) {
                map.setCenter(location);
                var marker = new google.maps.Marker({ map: map, position: location });
                clearPrewMarker(prewMarker);
                prewMarker = marker;
                map.setZoom(12);
            }

            function setMarkerForJanitorPlatform(location) {
                var newMap = getMapForSpecificAddress(location);
                var marker = new google.maps.Marker({
                    map: newMap,
                    position: location
                });
            }

            function getMapForSpecificAddress(location) {
                var options = {
                    center: location,
                    zoom: 12,
                    mapTypeId: google.maps.MapTypeId.ROADMAP
                };
                return new google.maps.Map(element[0].querySelector(".map-container"), options);
            }


            function placeMarkerAndPanTo(latLng, map) {
                var marker = new google.maps.Marker({
                    position: latLng,
                    map: map
                });

                clearPrewMarker(prewMarker);
                prewMarker = marker;
                map.panTo(latLng);

                var geocoder = new google.maps.Geocoder();
                geocoder.geocode({ 'latLng': latLng }, function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        scope.$apply(function () {
                            scope.onMapClicked({ address: results[0].formatted_address });
                        });
                    }
                });
            }

            function clearPrewMarker(marker) {
                if (marker) {
                    marker.setMap(null);
                }
            }

            initMap();
        }

        return self;
    }

    angular.module('boligdrift').directive('googleMap', [googleMap]);
})();