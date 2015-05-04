﻿function init() {
}

$(document).ready(function () {
    var vm = function () {
        var self = this

        self.messages = ko.observableArray([])
        self.isOpen = ko.observable(false)
        self.message = ko.observable("")
        self.canSend = ko.observable(true)
        self.details = ko.observable()
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
                if (result.success) {
                    self.isOpen(false)
                    var msg = message(result)
                    self.messages.unshift(msg)
                    self.message("")
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

        self.createComment = function (comment) {
            var reply = this;
            var data = {
                Text: comment.text,
                Parent_ID: comment.message_id,
                __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val()
            }
            $.post("/Messages/ReplyCreate", data, function (result) {
                if (result.success) {
                    var reply = reply(result)
                }
            })
        }

        self.test = function() {
            debugger
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