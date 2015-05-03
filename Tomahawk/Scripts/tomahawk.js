function init() {
}

$(document).ready(function () {
    var vm = function () {
        var self = this

        self.messages = ko.observableArray([])
        self.isOpen = ko.observable(false)
        self.message = ko.observable("")
        self.canSend = ko.observable(true)
        self.replies = ko.observableArray([])

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

        self.sendMessage = function (data) {
            $.post("/Messages/Create", { Text: data.message(), __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val() }, function (result) {
                self.isOpen(false)

                if (typeof result === "string") {
                    result = JSON.parse(result)
                }

                var msg = message(result)

                self.messages.push(msg)
            })
        }

        self.deleteMessage = function () {
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
            var reply = this;
            var data = {
                Text: comment.text,
                Parent_ID: comment.message_id,
                __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val()
            }
            $.post("/Messages/ReplyCreate", data, function (result) {
                if (typeof result === "string") {
                    result = JSON.parse(result)
                }

                var reply = reply(result)
            })
        }

        self.loadDetails = function() {
            var id = self.id()
            var name = self.name()
        }
    }

    var viewModel = new vm();

    pager.extendWithPage(viewModel)

    ko.applyBindings(viewModel, document.getElementById("wrapper"))

    pager.start()

    $.getJSON("/Messages/All", function (data) {
        viewModel.messages(data)
    //viewModel.createComment({text: "pickle", message_id: 1})
    })
})

var message = function (data) {
    return {
        id: data.ID,
        text: data.Text,
        name: data.User.UserName
    }
}

var reply = function (data) {
    return {
        id: data.ID,
        text: data.Text,
        name: data.User.UserName
    }
}