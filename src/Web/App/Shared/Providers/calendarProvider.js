var calendarProvider = function ($http, $q) {
    var baseUrl = '/api/calendar';
    var self = this;
    self.calendar = [];
    self.weekDates = [];

    var getCalendar = function (year) {
        var deferred = $q.defer();

        if (self.calendar[year]) {
            deferred.resolve(self.calendar[year]);
        } else {
            $http.get(baseUrl + '/getWeeksModel?year=' + year).then(function (result) {
                self.calendar[year] = result.data.monthWeeks;
                deferred.resolve(self.calendar[year]);
            });
        }

        return deferred.promise;
    }

    var getTotalWeeks = function () {
        var deferred = $q.defer();

        if (self.totalWeeks) {
            deferred.resolve(self.totalWeeks);
        } else {
            $http.get(baseUrl + '/getTotalWeeks').then(function (result) {
                self.totalWeeks = result.data;
                deferred.resolve(self.totalWeeks);
            });
        }

        return deferred.promise;
    }

    var getWeekDays = function (year, week) {
        var deferred = $q.defer();

        year = year || new Date().getFullYear();

        if (self.weekDates[year] && self.weekDates[year][week]) {
            deferred.resolve(self.weekDates[year][week]);
        } else {
            $http.get(baseUrl + '/getWeekDays?year=' + year + '&week=' + week).then(function (result) {
                if (!self.weekDates[year]) {
                    self.weekDates[year] = [];
                }
                self.weekDates[year][week] = result.data;
                deferred.resolve(self.weekDates[year][week]);
            });
        }

        return deferred.promise;
    }

    var getCurrentWeek = function () {
        var deferred = $q.defer();
        $http.get(baseUrl + '/GetCurrentWeek').then(function (result) {
            deferred.resolve(result.data);
        });
        return deferred.promise;
    }

    return {
        getCalendar: getCalendar,
        getTotalWeeks: getTotalWeeks,
        getWeekDays: getWeekDays,
        getCurrentWeek: getCurrentWeek
    };
};

angular.module('boligdrift').factory('calendarProvider', ['$http', '$q', calendarProvider]);