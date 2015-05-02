function init() {
    ko.applyBindings(vm, document.getElementById("wrapper"))
}

$(document).ready(init)

var vm = function () {
    var self = this

    self.isOpen = ko.observable(false)

    self.message = ko.observable("")

    self.canSend = ko.observable(true)

    self.getCharacterCount = ko.computed(function () {
        var limit = 10,
            count = self.message().length

        if (count > limit) {
            self.canSend(false)
        } else {
            self.canSend(true)
        }

        return (limit - count)
    })

    self.openMessage = function () {
        self.isOpen(true)
    }
}
