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
            var limit = 140,
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

        self.sendMessage = function(data) {
            $.post("/Messages/Create", { Text: data.message(), __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val() }, function (result) {
                self.isOpen(false)

                if (typeof result === "string") {
                    result = JSON.parse(result)
                }
                
                var msg = message(result)

                debugger

                self.messages.push(msg)
            })
        }

        self.deleteMessage = function() {
            var message = this;
            var data = { 
                id: message.id,
                __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val() 
            }
            $.post("/Messages/Delete", data, function (result) {
                if (result.status) {
                    self.messages.remove(message)
                } else {
                    alert("Nice try buddy")
                }
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

var message = function (data) {
    return {
        id: data.ID,
        text: data.Text,
        name: data.User.UserName
    }
}

var comment = function (id, text, username, date) {
    this.id = id;
    this.text = text;
    this.username = username;
    this.date = date;
}