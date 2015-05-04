$(document).ready(function () {
    var vm = function () {
        var self = this

        self.messages = ko.observableArray([])
        self.details = ko.observable()
        self.replies = ko.observableArray([])

        self.messageIsOpen = ko.observable(false)
        self.replyIsOpen = ko.observable(false)
        self.canSend = ko.observable(true)

        self.messageText = ko.observable("")

        self.getCharacterCount = ko.computed(function () {
            var limit = 140,
                count = self.messageText().length

            if (count > limit) {
                self.canSend(false)
            } else {
                self.canSend(true)
            }

            return (limit - count)
        })

        self.openMessage = function () {
            self.replyIsOpen(false)
            self.messageText("")
            self.messageIsOpen(true)
        }

        self.openReply = function () {
            self.messageIsOpen(false)
            self.messageText("")
            self.replyData = this
            self.replyIsOpen(true)
        }

        self.sendMessage = function (data) {
            $.post("/Messages/Create", { Text: data.messageText(), __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val() }, function (result) {
                if (result.success) {
                    self.messageIsOpen(false)
                    var msg = message(result)
                    self.messages.unshift(msg)
                    self.messageText("")
                }
            })
        }

        self.deleteReply = function () {
            var reply = this;
            var data = {
                id: reply.id,
                __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val()
            }
            $.post("/Messages/ReplyDelete", data, function (result) {
                if (result.success) {
                    self.replies.remove(reply)
                } else {
                    alert("Nice try buddy")
                }
            })
        }

        self.deleteMessage = function () {
            var message = this;
            var data = {
                id: message.id,
                __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val()
            }
            $.post("/Messages/Delete", data, function (result) {
                if (result.success) {
                    self.messages.remove(message)
                } else {
                    alert("Nice try buddy")
                }
            })
        }

        self.sendReply = function () {
            var data = {
                Text: self.messageText(),
                Parent_ID: self.replyData.id,
                __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val()
            }
            $.post("/Messages/ReplyCreate", data, function (result) {
                if (result.success) {
                    self.replyIsOpen(false)
                    self.replies.push(reply(result))
                    $('html, body').animate({
                        scrollTop: $("#reply_" + result.id).offset().top
                        }, 1000);
                    }
            })
        }

        self.loadDetails = function() {
            var id = self.id()
            var name = self.name()

            $.get("/Messages/Details/" + id, function (result) {
                if (result.success) {
                    self.details(result.message)
                    self.replies(result.replies)
                } else {
                    // TODO - this doesnt adjust the URL. figure out how to go back to start the right way
                    pager.goTo("start")
                }
            })
        }
    }

    var viewModel = new vm();

    pager.extendWithPage(viewModel)

    ko.applyBindings(viewModel, document.getElementById("wrapper"));

    pager.start()

    $.getJSON("/Messages/All", function (data) {
        if (typeof data === "object") {
            viewModel.messages(data)
        }
    })
})

var message = function (data) {
    return {
        id: data.id,
        text: data.text,
        name: data.name
    }
}

var reply = function (data) {
    return {
        id: data.id,
        text: data.text,
        name: data.name
    }
}