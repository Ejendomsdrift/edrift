(function () {
    var janitorService = function ($filter, moment, dateHelper) {
        var self = this;
        self.jobTypeNameList = [];
        var MINUTES_IN_HOUR = 60;
        var MILISECONDS_IN_MINUTE = 60000;

        self.isTaskOverdue = function (task) {
            return self.getTimeBeforeStartInMinutes(task.date) < 0;
        }

        self.isStartTimeLabelVisible = function (task) {
            return task.jobType === JobType.Tenant && task.jobStatus !== JobStatus.Completed;
        }

        self.getFormatedDateString = function (dateString) {
            return dateHelper.getLocalDateString(dateString);
        }

        self.getFormatedTimeString = function (time) {
            return dateHelper.getTimeString(time);
        }

        self.getEstimateTimeString = function (dateString, estimate) {
            var date = new Date(dateString);
            var dateWithEstimate = new Date(date.getTime() + estimate * MILISECONDS_IN_MINUTE);
            return dateHelper.getTimeString(dateWithEstimate);
        } 

        self.getEstimateTimeInHoursString = function (estimate) {
            var estimateInHours = estimate / MINUTES_IN_HOUR;
            return estimateInHours;
        } 

        self.getJobTypeName = function () {
            for (var name in JobType) {
                if (JobType.hasOwnProperty(name)) {
                    self.jobTypeNameList.push(name);
                }
            }

            return self.jobTypeNameList;
        }

        self.getJobStatusName = function () {
            return self.jobStatusNameList;
        }

        self.getTimeBeforeStartString = function (date) {
            var timeDiff = getTimeBeforeStart(date);

            var timeInMinutes = Math.abs(Math.round(timeDiff.minutes));
            var timeInHours = Math.abs(Math.round(timeDiff.hours));
            var timeInDays = Math.abs(Math.round(timeDiff.days));
            var timeInWeeks = Math.abs(Math.round(timeDiff.weeks));

            if (timeInWeeks >= 1 && timeInDays >= dateHelper.daysInWeek) {
                return getFormattedDateLabel(timeInWeeks, self.week, self.weeks);
            } else if (timeInDays >= 1 && timeInHours >= dateHelper.hoursInOneDay) {
                return getFormattedDateLabel(timeInDays, self.day, self.days);
            } else if (timeInHours >= 1 && timeInMinutes >= dateHelper.minutesInOneHour) {
                return getFormatedTimeLabel(timeInHours, timeInMinutes);
            } else {
                return getFormattedDateLabel(timeInMinutes, self.minute, self.minutes);
            }
        }

        self.getTimeBeforeStartLabel = function(task) {
            return self.isTaskOverdue(task) ? $filter('translate')('expired') : $filter('translate')('time before start');
        }

        self.scrollToTop = function () {
            window.scrollTo(0, 0);
        }

        self.getDaysAheadList = function () {
            var result = [
                { value: 1, text: '1 ' + self.day },
                { value: 2, text: '2 ' + self.days },
                { value: 3, text: '3 ' + self.days },
                { value: 4, text: '4 ' + self.days },
                { value: 5, text: '5 ' + self.days },
                { value: 6, text: '6 ' + self.days },
                { value: 7, text: '7 ' + self.days },
            ];

            return result;
        }

        self.getTimeBeforeStartInMinutes = function (date) {
            var timeDiff = getTimeBeforeStart(date);
            return Math.ceil(timeDiff.minutes);
        }

        function getTimeBeforeStart(date) {
            var taskStartDate = new Date(date);
            var todayDate = new Date();
            var timeDiff = dateHelper.getDifferenceBeetwenDates(taskStartDate, todayDate);
            return timeDiff;
        }

        function getFormattedDateLabel(number, singleText, multiText) {
            return number + ' ' + (number < 10 ? singleText : multiText);
        }

        function getFormatedTimeLabel(hours, minutes) {
            var timeInMinutes = minutes - (hours * MINUTES_IN_HOUR);

            if (timeInMinutes < 0) {
                hours = hours - 1; // compensate previous round e.g. 7.5 hours => 8 hours
                timeInMinutes = minutes - (hours * MINUTES_IN_HOUR);
            }

            var hoursString = hours + ' ' + (hours == 1 ? self.hour : self.hours);
            var minutesString = timeInMinutes + ' ' + (timeInMinutes < 10 ? self.minute : self.minutes);
            return hoursString + ' ' + minutesString;
        }

        function initTimeTranslation() {
            self.minute = $filter('translate')('minute');
            self.minutes = $filter('translate')('minutes');
            self.hour = $filter('translate')('hour');
            self.hours = $filter('translate')('hours');
            self.day = $filter('translate')('day');
            self.days = $filter('translate')('days');
            self.week = $filter('translate')('week');
            self.weeks = $filter('translate')('weeks');
        }

        function initService() {
            initTimeTranslation();
        }

        initService();

    };

    angular.module('boligdrift').service('janitorService', ['$filter', 'moment', 'dateHelper', janitorService]);
})();