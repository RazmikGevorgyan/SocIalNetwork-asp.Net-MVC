$(document).ready(function () {
    let us_id=$("#card-user").attr("data-id");
    let CurrentUserId = sessionStorage.getItem("id");

    $(document).on('click', '.panel-heading span.icon_minim', function (e) {
        var $this = $(this);
        if (!$this.hasClass('panel-collapsed')) {
            $this.parents('.panel').find('.panel-body').slideUp();
            $this.addClass('panel-collapsed');
            $this.removeClass('glyphicon-minus').addClass('glyphicon-plus');
        } else {
            $this.parents('.panel').find('.panel-body').slideDown();
            $this.removeClass('panel-collapsed');
            $this.removeClass('glyphicon-plus').addClass('glyphicon-minus');
        }
    });
    $(document).on('focus', '.panel-footer input.chat_input', function (e) {
        var $this = $(this);
        if ($('#minim_chat_window').hasClass('panel-collapsed')) {
            $this.parents('.panel').find('.panel-body').slideDown();
            $('#minim_chat_window').removeClass('panel-collapsed');
            $('#minim_chat_window').removeClass('glyphicon-plus').addClass('glyphicon-minus');
        }
    });
    function gotoBottom(id){
        var element = document.getElementById(id);
        element.scrollTop = element.scrollHeight;
    }
    $(document).on('click', '.icon_close', function (e) {
        $("#chatbox").css("display", "none")
    });
    let msgcheck = setInterval(function () {
        let arr=[];
        $.ajax({
            type: "post",
            url: "/Profile/GetMessages/",
            success: function (r) {
                r.forEach((item) => {
                    if(us_id==item.to_id){
                        arr.push(item.from_id);
                    }
                });
                var counts = {};
                arr.forEach(function(x){ counts[x] = (counts[x] || 0)+1; });
                for(var i in counts){
                    console.log(i,counts[i]);
                    $("#badge-"+i).html(counts[i]);
                }
            }

        });

    }, 3333);

    let intervalChat;
    $(".btn-message").click(function () {
        $(".msg_container_base").empty();
        $("#chatbox").css("display", "block")
        let id = $(this).attr("data-id");
        let name = $(this).attr("data-friendName");
        let surname = $(this).attr("data-friendSurname");
        $(".panel-title").html(name + " " + surname);
        $("#chat").attr('data-id', id);
        $.ajax({
            type:"post",
            url:"/Profile/MakeReaded/",
            data:{id1:id},
            success:function(){
                $("#badge-"+id).html("");
            }
        });
        clearInterval(intervalChat)
        intervalChat = setInterval(function () {
            $.ajax({
                type: "post",
                url: "/Profile/SeeMessages/" + id,
                success: function (r) {
                    r.forEach((item) => {
                        console.log($("#message-" + item.id).length)
                        if ($("#message-" + item.id).length == 0) {
                            let msgClass = ""
                            let photo = "";
                            let msg = $("<div id='message-" + item.id + "' class='row msg_container'></div>")
                            var ROOT = '@Url.Content("~")';
                            if (item.from_id == id) {
                                msg.addClass("base_receive")
                                msgClass = "msg_receive"
                                photo = item.from.replace('~', '')
                            } else {
                                msg.addClass("base_sent")
                                msgClass = "msg_sent";
                                photo = item.from.replace('~', '');
                            }
                            if (msgClass == "msg_receive") {
                                msg.append(`
                                <div class ="col-md-2 col-xs-2 avatar">
                                    <img  src="../../..${photo}" class ="rounded-circle"  style="height:50px;width:50px;"/>
                                </div>
                                    <div class ="col-md-10 col-xs-10">
                                    <div class ="messages ${msgClass} rounded text-left bg-light w-75 mr-auto">
                                        <p>
                                            ${item.content}
                                        </p>
                                        <time datetime="2009-11-13T20:00">${item.datetime}</time>
                                    </div>
                                </div>
                                `)
                            }
                            else {
                                msg.append(`
                                <div class ="col-md-10 col-xs-10">
                                    <div class ="messages ${msgClass} bg-primary rounded text-right w-75 ml-auto">
                                        <p>
                                            ${item.content}
                                        </p>
                                        <time datetime="2009-11-13T20:00">${item.datetime}</time>
                                    </div>
                                </div>
                                <div class ="col-md-2 col-xs-2 avatar float-right">
                                    <img  src="../../..${photo}" class ="rounded-circle"  style="height:50px;width:50px;" />
                                </div>
                                `)
                            }
                            msg.appendTo(".msg_container_base");
                        }
                    });
                }
            });
        }, 300);
    });

    $("#chat").click(function (event) {
        let id = $(this).attr("data-id");
        let text=new String($("#btn-input").val());
        $.ajax({
            type:"post",
            url: "/Profile/SendMessage",
            data: { id1: id, message: text },
            success: function(r){
                $("#btn-input").val("");
                gotoBottom('contain');
            }
        });
    })

    $(document).on("click",".btn-follow", function (){
        let id = $(this).attr("data-id");
        $.ajax({
            type: "post",
            url: "/Profile/AddFollow",
            data: { id1: id ,CurrentUser:CurrentUserId},
            success: function () {
                $(".fol").removeClass("btn-follow").removeClass("btn-success").addClass("btn-unfollow").addClass("btn-danger");
                $(".fol").html("unfollow");
            }
        });
    });
    $(document).on("click",".btn-unfollow", function (){
        let id = $(this).attr("data-id");
        $.ajax({
            type: "post",
            url: "/Profile/UnFollow",
            data: { id1: id, CurrentUser:CurrentUserId},
            success: function () {
                $(".fol").removeClass("btn-unfollow").removeClass("btn-danger").addClass("btn-follow").addClass("btn-success");
                $(".fol").html("follow");
            }
        });
    });
    $(document).on("click", ".btn-unfriend", function () {
        let id = $(this).attr("data-id");
        $.ajax({
            type: "post",
            url: "/Profile/UnFriend",
            data: { id1: id },
            success: function () {
                $(".friend-con").removeClass("btn-unfriend").addClass("btn-add-friend");
                $(".friend-con").html("Add Friend");
            }
        });

    });
    $(document).on("click",".btn-add-friend", function () {
        let id = $(this).attr("data-id");
        if ($("#addFriend-"+id).html() != "Deny") {
            $.ajax({
                type: "post",
                url: "/Profile/SendRequest/",
                data: { id1: id },
                success: function () {
                    $("#addFriend-" + id).html("Deny");
                    $("#addFriend-" + id).removeClass(".btn-primary").addClass(".btn-danger");
                    alert("oks")
                }
            });
        } else {
            $.ajax({
                type: "post",
                url: "/Profile/DenyYourRequest",
                data: { id1: id},
                success: function (r) {
                    $("#addFriend-" + id).html("Add Friend")
                    $("#addFriend-" + id).removeClass(".btn-danger").addClass(".btn-primary");
                }
            });
        }
    });

    $(".btn-add-Confim").click(function () {
        let id = $(this).attr("data-id");
        $.ajax({
            type: "post",
            url: "/Profile/ConfimRequest",
            data: { id1: id },
            success: function (r) {
                $(".btn-follow").html("")
            }
        });
    });

    $(".log-out").click(function () {
        
        $.ajax({
            type: "post",
            url: "/User/ClearCoockies/"
        })
        sessionStorage.clear();
    });
    $(document).on("click", ".block-user", function () {
        let id = $(this).attr("data-user-id");
        $.ajax({
            type: "post",
            url: "/Admin/Block/" + id,
            success: function () {
                $(".block-user").empty();
                $(".block-user").append(`<img src="https://img.pngio.com/access-account-avatar-block-blocked-denied-user-icon-blocked-png-512_512.png" style="height:30px;width:30px;"/>`)
                $(".block-user").addClass("reblock-user").addClass("btn-primary").removeClass("block-user").removeClass("btn-danger");
            },
        });
    })
    $(document).on("click",".reblock-user", function () {
        let id = $(this).attr("data-user-id");
        $.ajax({
            type: "post",
            url: "/Admin/UnBlock/" + id,
            success: function () {
                $(".reblock-user").empty();
                $(".reblock-user").append(`<img src="https://static.thenounproject.com/png/574710-200.png" style="height:30px;width:30px;"/>`)
                $(".reblock-user").addClass("block-user").addClass("btn-danger").removeClass("reblock-user").removeClass("btn-primary");
            },
        });
    })
    $(window).scroll(function () {
        var scrollYpos = $(document).scrollTop();
        if (scrollYpos >= 40) {
            $("#advertToll").css({
                'top': '10px',
                'position': 'fixed',
                'transition':'0.1s'
            });
        } else 
        {
            $("#advertToll").css({
                'top': '60px',
                'position': 'absolute',
            });
        }
    });
});