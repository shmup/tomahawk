$(document).ready(function () {
    var vm = function () {
        var self = this

        self.loggedIn = ko.observable(false)

        if ($("#authorized").length) {
            self.loggedIn(JSON.parse($("#authorized").val()))
        }

        self.username = $("#username").val()
        self.messages = ko.observableArray([])
        self.details = ko.observable()
        self.replies = ko.observableArray([])
        self.spinner = newSpinner()
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

        self.sendMessage = function (data) {
            $.post("/Messages/Create", { Text: data.messageText(), __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val() }, function (result) {
                if (result.success) {
                    self.messageIsOpen(false)
                    var msg = message(result)
                    self.messages.unshift(msg)
                    self.messageText("")
                    highlight($("#message_" +msg.id));
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
                    pager.goTo("start")
                } else {
                    alert("Nice try buddy")
                }
            })
        }

        self.openReply = function (cool) {
            self.messageIsOpen(false)
            self.messageText("")
            self.replyData = this
            self.replyIsOpen(true)
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
                    var replyElement = $("#reply_" + result.id)
                    $('html, body').animate({
                        scrollTop: replyElement.offset().top
                        }, 1000);
                    }
                    highlight(replyElement)
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

        self.spinnerLoader = function(page, element) {
            var loader = {};
            var txt = $('<img src="http://pagerjs.com/demo/ajax-loader.gif"/>');
            loader.load = function () {
                $(element).empty();
                $(element).append(txt);
            };
            loader.unload = function () {
                txt.remove();
                };
            return loader;
        }

        self.cleanUp = function() {
            self.messageIsOpen(false)
            self.replyIsOpen(false)
            self.messageText("")

            self.spinner.spin()

            var data = {}
            var url = "/Messages/All";

            if (typeof self.nick() !== "undefined" && location.hash.indexOf("#start") < 0) {
                url = "/Messages/UserAll"
                data["user"] = self.nick()
            }

            $.getJSON(url, data, function (data) {
                if (typeof data === "object") {
                    self.messages(data)
                    self.spinner.stop()
                }
            })
        }

        self.loadDetails = function() {
            var id = self.id()
            var name = self.name()

            self.replies([])

            self.spinner.spin()

            $.get("/Messages/Details/" + id, function (result) {
                if (result.success) {
                    self.spinner.stop()
                    self.details(result.message)
                    self.replies(result.replies)
                    if (typeof self.reply() !== "undefined") {
                        self.openReply()
                        $(".new-reply textarea").focus()
                    }
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

    $(".body-content").append(viewModel.spinner.el);
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

var newSpinner = function () {
    var opts = {
        lines: 13, // The number of lines to draw
        length: 20, // The length of each line
        width: 10, // The line thickness
        radius: 30, // The radius of the inner circle
        corners: 1, // Corner roundness (0..1)
        rotate: 0, // The rotation offset
        direction: 1, // 1: clockwise, -1: counterclockwise
        color: '#000', // #rgb or #rrggbb or array of colors
        speed: 1, // Rounds per second
        trail: 60, // Afterglow percentage
        shadow: false, // Whether to render a shadow
        hwaccel: false, // Whether to use hardware acceleration
        className: 'spinner', // The CSS class to assign to the spinner
        zIndex: 2e9, // The z-index (defaults to 2000000000)
        top: '50%', // Top position relative to parent
        left: '50%' // Left position relative to parent
    };
    return new Spinner(opts);
}

var highlight = function (elem) {
    $(elem).effect("highlight", { }, 1500);
}