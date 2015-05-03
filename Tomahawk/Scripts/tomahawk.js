function init() {
}

$(document).ready(function () {
    var vm = function () {
        var self = this

        self.messages = ko.observableArray([])

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

        self.createMessage = function(msg) {
            $.post("/Messages/Create", { Text: msg, __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val() }, function (result) {
                debugger;
            })
        }

        self.createComment = function (comment) {
            $.post("/Messages/Create", { Text: "lol", __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val() }, function (result) {
                debugger;
            })
        }
    }

    var viewModel = new vm();

    ko.applyBindings(viewModel, document.getElementById("wrapper"))

    $.getJSON("/Messages/All", function (data) {
        viewModel.messages(data)
    })
})

var message = function (id, text, username, date) {
    this.id = id;
    this.text = text;
    this.username = username;
    this.date = date;
    this.comments = ko.observableArray([]);
}

var comment = function (id, text, username, date) {
    this.id = id;
    this.text = text;
    this.username = username;
    this.date = date;
}