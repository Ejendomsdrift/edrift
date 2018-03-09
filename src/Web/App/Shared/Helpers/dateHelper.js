(function () {
    var dateHelper = function ($filter, moment) {
        var self = this;
        self.milisecondsInOneSecond = 1000;
        self.secondsInOneMinute = 60;
        self.minutesInOneHour = 60;
        self.hoursInOneDay = 24;
        self.daysInWeek = 7;

        self.parseDateFromDatePickerFormat = function (dateString) {
            return moment.utc(dateString, DatePickerFormatType.dayMonthYear.toUpperCase());
        }

        self.formatUtcDateString = function (utcDateString) { //please note that here date come already in UTC format
            if (!utcDateString) {
                return '';
            }

            if (typeof (date) === 'string') {
                utcDateString = new Date(utcDateString);
            }

            var momentDate = moment(utcDateString);
            return momentDate.format(DatePickerFormatType.dayMonthYear.toUpperCase());
        }

        self.getLocalDateString = function (date) {
            if (!date) {
                return '';
            }

            if (typeof (date) === 'string') {
                date = new Date(date);
            }

            var momentDate = moment.utc(date);
            return momentDate.format(DatePickerFormatType.dayMonthYear.toUpperCase());
        }

        self.getTimeString = function (dateString) {
            var date = new Date(dateString);
            var hours = date.getHours().toString();
            var minutes = date.getMinutes().toString();
            return self.getTimeStringByHousrAndMinutes(hours, minutes);
        }

        self.getTimeStringByHousrAndMinutes = function(hours, minutes) {
            hours = hours.length == 1 ? '0' + hours : hours;
            minutes = minutes.length == 1 ? '0' + minutes : minutes;
            return hours + ':' + minutes;
        }

        self.formatSpentTimeFromHoursAndMinutes = function (spentHours, spentMinutes) {
            var hours = spentHours.toString();
            var minutes = spentMinutes.toString();
            var modifiedHours = hours.length == 1 ? '0' + hours : hours;
            var modifiedMinutes = minutes.length == 1 ? '0' + minutes : minutes;
            return modifiedHours + ' : ' + modifiedMinutes;
        }

        self.parseYearFromDatePickerFormat = function (year) {
            return moment.utc({ year: year, month: 0, day: 1 });
        }

        self.dateFromYearWeekDay = function (year, week, dayOfWeek) {
            var date = moment.utc({ year: year, month: 0, day: 1 })
                .add(week - 1, 'weeks')
                .add(dayOfWeek, 'days');
            return date;
        }

        self.minutesToViewFormat = function minutesToViewFormat(time) {
            var hourLabel = $filter('translate')('timeViewFormat_hour_short');

            if (!time || time < 1) {
                return "0" + hourLabel;
            }

            var minutesInHour = 60;
            var hours = Math.floor(time / minutesInHour);
            time -= hours * minutesInHour;

            var minutes = time;
            var result = "";

            if (hours > 0) {
                result += hours + hourLabel + " ";
            }

            if (minutes > 0) {
                var minuteLabel = $filter('translate')('timeViewFormat_minute_short');
                result += minutes + minuteLabel;
            }

            return result;
        }

        self.getDifferenceBeetwenDates = function (firstDate, secondDate) {
            var result = {};
            result.miliseconds = firstDate.getTime() - secondDate.getTime();
            result.seconds = result.miliseconds / self.milisecondsInOneSecond;
            result.minutes = result.seconds / self.secondsInOneMinute;
            result.hours = result.minutes / self.minutesInOneHour;
            result.days = result.hours / self.hoursInOneDay;
            result.weeks = result.days / self.daysInWeek;

            return result;
        }

        self.getCurrentWeekNumber = function () {
            var date = new Date();
            date.setHours(0, 0, 0, 0);
            date.setDate(date.getDate() + 4 - (date.getDay() || 7));
            return Math.ceil((((date - new Date(date.getFullYear(), 0, 1)) / 8.64e7) + 1) / 7);
        }

        self.getTotalMillisecondsFromDateAndTime = function (date, time) { 
            if (!moment.isMoment(date)) {
                console.error('Date should be moment object.');
                return 0;
            }

            if (!moment.isMoment(time)) {
                console.error('Time should be moment object.');
                return 0;
            }

            var year = date.year();
            var month = date.month();
            var day = date.date();
            var hours = time.hour();
            var minutes = time.minute();
            return new Date(year, month, day, hours, minutes).getTime();
        }
    };

    angular.module('boligdrift').service('dateHelper', ['$filter', 'moment', dateHelper]);
})();