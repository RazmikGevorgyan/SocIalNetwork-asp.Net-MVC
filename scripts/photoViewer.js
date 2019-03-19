$(document).ready(function (){
    let CurrentUserId = sessionStorage.getItem("id");
    function GetCommentForPhoto(feedId, counter) {
        let CurrentUserId = sessionStorage.getItem("id");
        $(".for-comment").empty();
        $(".for-comment").append(`<a id="view-more" data-id="${feedId}" style="cursor:pointer" data-counter="${counter}" class="text-primary">view more</a>`);
        $.ajax({
            type: "post",
            url: "/Profile/GetComments/" + feedId,
            data:{counter:counter},
            success: function (r) {
                r.forEach((item) => {
                    let name = item.name;
                    let surname = item.surname;
                    let data = item.datetime;
                    let commentator_id = item.commentator;
                    let context = item.comment;
                    let photo = item.photo;
                    photo = photo.replace('~', '');
                    let src = "../.." + photo;
                    $(".for-comment").append(`
                            <div class ="row">
                                    <div class ="col-3">
                                        <img src="${src}" style="width:30px;height:30px;"  class ="rounded-circle float-right mr-1">
                                    </div>
                                    <div class ="col-9">
                                        <label class ="rounded w-100" style="background-color: rgba(0, 255, 255, 0.3)">
                                                <a href="/Chrono/index/${commentator_id}">${name} ${surname}</a>
                                                <br>
                                                ${context}
                                        </label>
                                    </div>
                            </div>
                        `);
                });
            },
        });
    }
 
    $(document).on('click', '#view-more', function () {
        let feedId = $(this).attr("data-id");
        let counter = new Number($(this).attr("data-counter"));
        if (counter.toString() == "NaN") {
            counter = 5;
        }
        counter += 5;
        GetCommentForPhoto(feedId, counter);
    });
  
    $(document).on('click', '.viewer', function () {
        let CurrentUserId = sessionStorage.getItem("id");
        let user_id = $(this).attr("data-id");
        let feedId = $(this).attr("data-feedId");
        let counter = 5;
        $(".commentOn").attr("data-id", feedId);
        $(".carousel-control-next").attr("feed-id", feedId);
        $(".carousel-control-prev").attr("feed-id", feedId);
        $.ajax({
            type: "post",
            url: "/Chrono/GetOnePhoto/" + feedId,
            success: function (r) {
                $(".carousel-inner").empty();
                let name = r.name;
                let surname = r.surname;
                let description = r.description;
                if (description == undefined) {
                    description = "";
                }
                let likes = r.likes;
                let profile_photo = r.profile_photo;
                let photo = r.photo;
                let video = r.video;
                let src;
                profile_photo = profile_photo.replace('~', '');
                let sr = "../.." + profile_photo;
                if (r.photo != null) {
                    photo = photo.replace('~', '');
                    src = "../.." + photo;
                    $(".carousel-inner").append(`
                        <div class ="carousel-item active">
                            <img src="${src}"  width="460" height="600">
                        </div>
                    `);
                } else {
                    video = video.replace('~', '');
                    srcVid = "../.." + video;
                    src = "../.." + photo;
                    $(".carousel-inner").append(`
                        <div class ="carousel-item active">
                            <video src="${src}" width="460" height="600">
                        </div>
                    `);
                }
                $(".user_profile").empty();
                $(".user_profile").append(`
                    <img src="${sr}" width="40" height="40"  class ="rounded-circle">
                `)
                $(".user_name").empty();
                $(".user_name").append(`
                    <strong><a href="/Chrono/index/${user_id}">${name} ${surname}</a></strong>
                    `)
                $(".user_name").append(`<div class="photo-descr"></div>`)
                $(".photo-descr").append(`
                    <hr>
                      <div class ="row">
                            <img class ="like" data-id="${feedId}" alt="like" style="height:20px;width:20px;" src="../../Content/Backgrounds/like.png" />
                           <p class ="float-left ml-1"> ${likes}</p>
                      </div>
                      <div class ="container h-20 w-100" style="overflow:hidden">
                           <label class ="text-left">${description}<label>
                       </div>
                `)
                GetCommentForPhoto(feedId, 5);
            },
        });
    });

    $(".carousel-control-prev").click(function () {
        let currentFedd = $(".carousel-control-prev").attr("feed-id");
        $.ajax({
            type: "post",
            url: "/Chrono/GetPrevItem/" + currentFedd,
            success: function (r) {
                if (r.id != currentFedd) {
                    $(".carousel-inner").empty();
                    let feId = r.feedId;
                    $(".commentOn").attr("data-id", feId);
                    $(".carousel-control-next").attr("feed-id", feId);
                    $(".carousel-control-prev").attr("feed-id", feId);
                    let photo = r.photo;
                    let video = r.video;
                    let src;
                    let description = r.description;
                    if (description == undefined) {
                        description = "";
                    }
                    let likes = r.likes;
                    if (r.photo != null) {
                        photo = photo.replace('~', '');
                        src = "../.." + photo;
                        $(".carousel-inner").append(`
                            <div class ="carousel-item active">
                                <img src="${src}" width="460" height="600">
                            </div>
                        `);
                    } else {
                        video = video.replace('~', '');
                        src = "../.." + video;
                        $(".carousel-inner").append(`
                            <div class ="carousel-item active">
                                <video src="${src}" width="460" height="600">
                            </div>
                        `);
                    }
                    $(".photo-descr").empty();
                    $(".photo-descr").append(`
                      <hr>
                      <div class ="row">
                           <img class ="like" data-id="${feId}" alt="like" style="height:20px;width:20px;" src="../../Content/Backgrounds/like.png" />
                           <p class ="float-left ml-1"> ${likes}</p>
                      </div>
                      <div class ="container h-25 w-100" style="overflow:hidden">
                           <label class ="text-left">${description}<label>
                       </div>
                    `)
                }
            }
        });

    });

    $(".carousel-control-next").click(function () {
        let currentFedd = $(".carousel-control-next").attr("feed-id");
        $.ajax({
            type: "post",
            url: "/Chrono/GetNextItem/" + currentFedd,
            success: function (r) {
                if (r.id != currentFedd) {
                    $(".carousel-inner").empty();
                    let feedId = r.feedId;
                    $(".commentOn").attr("data-id", feedId);
                    GetCommentForPhoto(feedId,5);
                    $(".carousel-control-next").attr("feed-id", feedId);
                    $(".carousel-control-prev").attr("feed-id", feedId);
                    let photo = r.photo;
                    let video = r.video;
                    let description = r.description;
                    if (description == undefined) {
                        description = "";
                    }
                    let likes = r.likes;
                    let src;
                    if (r.photo != null) {
                        photo = photo.replace('~', '');
                        src = "../.." + photo;
                        $(".carousel-inner").append(`
                            <div class ="carousel-item active">
                                <img src="${src}" width="460" height="600">
                            </div>
                        `);
                    } else {
                        video = video.replace('~', '');
                        src = "../.." + video;
                        $(".carousel-inner").append(`
                            <div class ="carousel-item active">
                                <video src="${src}" width="460" height="600">
                            </div>
                        `);
                    }
                    $(".photo-descr").empty();
                    $(".photo-descr").append(`
                            <hr>
                            <div class="row">
                                 <img class ="like" data-id="${feedId}" alt="like" style="height:20px;width:20px;" src="../../Content/Backgrounds/like.png" />
                                 <p class ="float-left ml-1"> ${likes}</p>
                            </div>
                            <div class="container h-25 w-100" style="overflow:hidden">
                                  <label class ="text-left">${description}<label>
                            </div>
                    `)
                }
            }
        });
    });
    $(document).on('click', '.like', function () {
        let id = new Number($(this).attr("data-id"));
        let liked =new String($(this).attr("data-liked"));
        if (liked == "non-liked") {
            $.ajax({
                type: "post",
                url: "/Profile/AddLike/" + id,
                success: function () {
                    $(".like-icon-"+id).empty();
                    $(".like-icon-"+id).append(`
                            <img class ="like ml-auto" data-id="${id}" data-liked="liked" alt="like" style="height:19px;width:19px;" src="../../Content/Backgrounds/likeYes.png" />
                        `);
                }
            });
        }
        else {
            $.ajax({
                type: "post",
                url: "/Profile/DeleteLike/" + id,
                success: function () {
                    $(".like-icon-"+id).empty();
                    $(".like-icon-"+id).append(`
                            <img class ="like ml-auto" data-id="${id}" data-liked="non-liked" alt="like" style="height:19px;width:19px;" src="../../Content/Backgrounds/likeNew.png" />
                        `);
                }
            });
        }
    });
    let intervalcomment;
    $(".comment").click(function () {
        let state = $(this).attr("aria-expanded");
        $(".comment").attr("aria-expanded","false")
        if (state.toString() == "false") {
            $(".collapse").removeClass("show");
        }
            let newsId = $(this).attr("data-id");
            GetCommentForPhoto(newsId, 5);

    });

    $(".Com").click(function () {

        let newsId = $(this).attr("data-id")
        let content = new String($("#comment-text-" + newsId).val());
        if (content != "" && content != null && content != undefined) {
            $.ajax({
                type: "post",
                url: "/Profile/AddComment/" + newsId,
                data: { content: content },
                success: function () {
                    GetCommentForPhoto(newsId, 5);
                },
            });
        }
    });

    $(".Confim").click(function () {
        let id = $(this).attr("data-id");
        let notifID = $(this).attr("data-notif-id");
        let nameSur = $(".names").attr("data-name");
        $.ajax({
            type: "post",
            url: "/Profile/ConfimRequest/",
            data: { id: id, notifID: notifID,userId:CurrentUserId },
            success: function (r) {
                $(".Confim").closest("div").html("you and " + nameSur + " are friends now");
                $(".Confim").closest(".dropdown-menu").toggle;
            }
        });
    });
    $(".Deny").click(function () {
        let notifID = $(this).attr("data-notif-id");
        $.ajax({
            type: "post",
            url: "/Profile/DenyRequest/",
            data: { notifID: notifID},
            success: function (r) {
                $(".Confim").closest("div").html("");
                $(".Confim").closest("div .dropdown-menu").toggle;
            }
        });
    });
    $(".notif").click(function () {
        let num = new String($(".badgeNotif").html())
        if (num != "") {
            $.ajax({
                type: "post",
                url: "/Profile/ClearNotifications/",
                success: function (r) {
                    $(".badgeNotif").html("");
                }
            });
        }
    });
    $(".commentOn").click(function () {
        let feedId = $(this).attr("data-id");
        let content = new String($(".comment-input").val());
        if (content != null || content != undefined || content != "")
            {
            $.ajax({
                type: "post",
                url: "/Profile/AddComment/" + feedId,
                data:{content:content},
                success: function (r) {
                GetCommentForPhoto(feedId,5);
                },
            });
            }
        });
    });
