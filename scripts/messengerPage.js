$(document).ready(function () {
    let us_id =new String( $.cookie("id"));
    let msgcheck = setInterval(function () {
        let arr = [];
        $.ajax({
            type: "post",
            url: "/Profile/GetMessages/",
            success: function (r) {
                r.forEach((item) => {
                    if (us_id == item.to_id) {
                        arr.push(item.from_id);
                    }
                });
                var counts = {};
                arr.forEach(function (x) { counts[x] = (counts[x] || 0) + 1; });
                for (var i in counts) {
                    $("#badge-" + i).html(counts[i]);
                }
            }

        });

    }, 3333);
    let intervalChat;
    $(document).on ('click','.card-of-user',function () {
        $(".card-of-user").removeClass("active");
        $(this).addClass("active");
        $(".msg_container_base").empty();
        let id = $(this).attr("data-id");
        let name = $(this).attr("data-name");
        let surname = $(this).attr("data-surname");
        $(".user_info-bage").empty();
        $(".user_info-bage").append(`<span>Chat with ${name}</span>`)
        $.ajax({
            type: "post",
            url: "/Profile/MakeReaded/",
            data: { id1: id},
            success: function () {
                $("#badge-" + id).html("");
                $(".send_btn").attr("data-id", id);
            }
        });
        clearInterval(intervalChat)
        intervalChat = setInterval(function () {
            $.ajax({
                type: "post",
                url: "/Profile/SeeMessages/" + id,
                success: function (r) {
                    r.forEach((item) =>{
                        if ($("#message-" + item.id).length == 0) {
                            let msg = $("<div id='message-" + item.id + "'class=''></div>")
                            var ROOT = '@Url.Content("~")';
                            let photo = "";
                            if (item.from_id == id) {
                                msg.addClass("d-flex justify-content-start mb-4")
                                photo = item.from.replace('~', '');
                            } else {
                                msg.addClass("d-flex justify-content-end mb-4")
                                photo = item.from.replace('~', '');
                            }
                            if (msg.hasClass("justify-content-start")) {
                                msg.append(`
                                <div class ="img_cont_msg">
                                    <img  src="../../..${photo}" class ="rounded-circle  user_img_msg"/>
                                </div>
                                <div class ="msg_cotainer">
                                    <p>
                                        ${item.content}
                                    </p>
                                    <time datetime="2009-11-13T20:00">${item.datetime}</time>
                                </div>
                                `)
                            }
                            else {
                                msg.append(`
                                <div class ="msg_cotainer_send">
                                    <p>
                                        ${item.content}
                                    </p>
                                    <time datetime="2009-11-13T20:00">${item.datetime}</time>
                                </div>
                                 <div class ="img_cont_msg">
                                    <img  src="../../..${photo}" class ="rounded-circle  user_img_msg"/>
                                </div>
                                `)
                            }
                            $(".msg_container_base").scrollTop(1000000000000000000);
                            msg.appendTo(".msg_container_base");
                        }
                    });
                }
            });
        }, 300);
    });
    let Interval;
    $(document).on("change",".search", function () {
            clearInterval(Interval);
    });
    $(document).on('click', '.search', function () {
        clearInterval(Interval);
        Interval = setInterval(function () {
            let text = new String($(".search").val());
            if (text != "" || text != null) {
                let url = "";
                if (us_id == 25) {
                   url = "/Admin/SearcheResult/";
                } else {
                   url = "/Profile/SearcheResult/";
                }
            $.ajax({
                type:"post",
                url: url,
                data:{text:text},
                success: function (r) {
                    if (r.length != 0) {
                        $(".contacts").empty();
                        r.forEach((item) => {
                            let name = item.name;
                            let surname = item.surname;
                            let id = item.id;
                            let photo=item.photo.replace("~","");
                            if ($("#user-" + id).length == 0) {
                                $(".contacts").append(`
                                      <li class ="card-of-user" data-name="${name}" data-surnae="${surname}" data-id="${id}">
                                        <div class ="d-flex bd-highlight">
                                            <div class ="img_cont">
                                                <img src="../../${photo}" class ="rounded-circle user_img">
                                            </div>
                                            <div id="user-${id}" class ="user_info">
                                                  <span>${name} ${surname} <span id="badge-${id}" class ="badge badge-danger"></span></span>
                                             </div>
                                        </div>
                                      </li>
                                    `)
                            }
                        });
                    }
                },
            });
            } 
       }, 1000);
    });
    
    $(".send_btn").click(function (event) {
        let id = $(this).attr("data-id");
        let text = new String($(".type_msg").val());
        $.ajax({
            type: "post",
            url: "/Profile/SendMessage",
            data: { id1: id, message: text },
            success: function (r) {
                $(".type_msg").val("");
            }
        });
    });
});