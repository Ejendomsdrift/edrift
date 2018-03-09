var BasePipeline = (function () {
    function BasePipeline() {
        this.defaultMaxTimes = 1;
        this.defaultCurrentMaxTimes = 0;
        this.items = [];
    }
    BasePipeline.prototype.add = function (item) {
        if (!item.maxTimes) {
            item.maxTimes = this.defaultMaxTimes;
        }
        if (!item.currentTimes) {
            item.currentTimes = this.defaultCurrentMaxTimes;
        }
        if (!item.rules) {
            item.rules = [];
        }
        this.items.push(item);
    };
    BasePipeline.prototype.tryNext = function (currentType) {
        if (!this.isRunNextItem(currentType)) {
            return PipelineResultEnum.RulesNotPass;
        }
        var nextItem = this.getNextItem(currentType);
        if (nextItem.currentTimes >= nextItem.maxTimes || !nextItem.run) {
            return PipelineResultEnum.ReachedMaxTimesOrRunFunctionIsMissing;
        }
        nextItem.currentTimes++;
        nextItem.run();
        return PipelineResultEnum.Success;
    };
    BasePipeline.prototype.isRunNextItem = function (currentType) {
        var currentItem = this.getCurrentItem(currentType);
        var result = this.isPipelineRulesValid(currentItem);
        return result;
    };
    BasePipeline.prototype.isPipelineRulesValid = function (nextItem) {
        var result = _.every(nextItem.rules, function (rule) {
            return rule.check();
        });
        return result;
    };
    BasePipeline.prototype.getCurrentItem = function (currentType) {
        var result = this.getItem(currentType);
        return result;
    };
    BasePipeline.prototype.getNextItem = function (currentType) {
        var nextItemPipepileType = this.getNextPipelineItem(currentType);
        var result = this.getItem(nextItemPipepileType);
        return result;
    };
    BasePipeline.prototype.getItem = function (type) {
        var result = _.find(this.items, function (pipelineItem) {
            return pipelineItem.type === type;
        });
        return result;
    };
    return BasePipeline;
}());
var PipelineCounterRule = (function () {
    function PipelineCounterRule(getTotalCount) {
        this.getTotalCount = getTotalCount;
        this.counter = 0;
    }
    PipelineCounterRule.prototype.check = function () {
        var totalCount = this.getTotalCount();
        if (totalCount === 0) {
            return true;
        }
        this.counter++;
        return this.counter === totalCount;
    };
    return PipelineCounterRule;
}());
var PipelineResultEnum;
(function (PipelineResultEnum) {
    // success move to next pipeline item
    PipelineResultEnum[PipelineResultEnum["Success"] = 0] = "Success";
    // some rule return false
    PipelineResultEnum[PipelineResultEnum["RulesNotPass"] = 1] = "RulesNotPass";
    // currentTimes >= maxTimes
    PipelineResultEnum[PipelineResultEnum["ReachedMaxTimesOrRunFunctionIsMissing"] = 2] = "ReachedMaxTimesOrRunFunctionIsMissing";
})(PipelineResultEnum || (PipelineResultEnum = {}));
//# sourceMappingURL=pipeline.js.map