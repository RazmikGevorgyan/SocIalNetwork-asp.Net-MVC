$(document).ready(function () {
    let sources=[];
    $("#file-input").change(function (e) {
        var files = e.target.files;
        let CurrentUserId = sessionStorage.getItem("id");
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    data.append("file" + x, files[x]);
                }
                $(".picture-area").empty();
                $.ajax({
                    type: "POST",
                    url: "/Profile/UploadFile/",
                    contentType: false,
                    processData: false,
                    data:  data ,
                    success: function (result) {
                        let counter = 0
                        result.forEach(item=>{
                            if (item.photo != "") {
                                let source = item.photo
                                sources.push(source);
                                let src = item.photo;
                                src = src.replace('~', '');
                                src = "../.." + src;
                                $(".picture-area").append(`
                                    <div id="card-no-${counter}" class ="card col-md-4 h-100 w-100">
                                         <div class ="card-header btn btn-primary data-close-feed text-center"  data-id="${counter}" data-src="${source}" style="height:30px;">&times;</div>
                                         <img  class ="card-img-top content-src" data-src="${src}" style="height:150px;" src="${src}"/>
                                    </div>
                            `)
                                $(".share-area").empty();
                                $(".share-area").append(`
                             `)
                            }
                            else {
                                let source = item.photo
                                sources.push(source);
                                let src = item.video;
                                src = src.replace('~', '');
                                src = "../.." + src;
                                $(".picture-area").append(`
                                    <div id="card-no-${counter}" class ="card col-md-4 h-100 w-100">
                                         <div class ="card-header btn btn-primary data-close-feed text-center"  data-id="${counter}" data-src="${source}" style="height:40px;"> <span aria-hidden="true">&times; </span></div>
                                         <video  class ="card-img-top content-src"  style="height:150px;" src="${src}"></video>
                                    </div>
                            `)
                            }
                                                    
                            counter++;
                        });
                        $(".share-area").append(
                        `<input class ="btn btn-primary" id="share-all" type="submit" name="send" value="Share" />`
                            )
                    },
                });
            } else {
                alert("Something went wrong! Please try again.");
            }
        }
    });
    function getId(url) {
        var regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*/;
        var match = url.match(regExp);
        if (match && match[2].length == 11) {
            return match[2];
        } else {
            return 'error';
        }
    }
    $(".url-post").change(function () {
        let CurrentUserId = sessionStorage.getItem("id");
        let url = new String($(this).val());
        let newUrl = getId(url);
        let counter = 0;
        $(".picture-area").empty();
        var iframeMarkup = '<iframe width="100%" height="100%" class="frameVid"  src="//www.youtube.com/embed/'
            + newUrl + '"frameborder="0" allowfullscreen></iframe>';
        $(".picture-area").append(`
            <div id="card-no-${counter}" class ="card col-md-12 h-100 w-100">
                    <div class ="card-header btn btn-primary data-close-feed-url text-center" data-id="${counter}"  aria-label="Close"  style="height:40px;"> <span class="mb-2" aria-hidden="true">&times; </span> </div>
                    <div class="card-body" style="height:400px;"> ${iframeMarkup}</div>
            </div>
        `)
        $(".share-area").empty();
        $(".share-area").append(`
            <input class ="btn btn-primary" id="share-url" type="submit" name="send" value="Share" />
        `)
    });
    $(document).on('click', '#share-url', function () {
        let CurrentUserId = sessionStorage.getItem("id");
        let url = new String($(".frameVid").attr("src"));
        let text = new String($(".area").val());
        if (url != "" || url != undefined || url != null) {
            $.ajax({
                type: "post",
                url: "/Chrono/setUrl/",
                data: { url: url,text:text},
                success: function () {
                    $(".area").val("");
                    $(".url-post").val("");
                    $(".picture-area").empty();
                    $(".picture-area").append(`<label class="text-success">successfuly done</label>`);
                    location.reload();
                },
            });
        }
    });
    $(document).on('click','.data-close-feed-url', function () {
        let cardid = $(this).attr("data-id");
            $("#card-no-" + cardid).remove();
            $(".url-post").val("");
            if (!$.trim($('.picture-area').html()).length) {
                $(".share-area").empty();
            }
        });
    $(document).on('click', '.data-close-feed', function () {
        let cardid = $(this).attr("data-id");
        let path = $(this).attr("data-src");
        let CurrentUserId = sessionStorage.getItem("id");
        $.ajax({
            type: "post",
            url: "/Chrono/deletPhoto/" ,
            data:{path:path},
            success: function () {
                $("#card-no-" + cardid).remove();
                if (!$.trim($('.picture-area').html()).length) {
                   $(".share-area").empty();
                }
            },
        });
    });

    //$(document).on('click', '.data-close', function () {
    //    let feedId = $(this).attr("data-id");
    //    let path = $(this).attr("src");
    //    $.ajax({
    //        type: "post",
    //        url: "/Chrono/deleteFeedWithPhoto/" + feedId,
    //        data:{src:path},
    //        success: function () {
    //            $("#card-no-"+feedId).remove();
    //        },
    //    });
    //});
    window.onbeforeunload = function (event) {
        if (sources.length != 0) {
            let CurrentUserId = sessionStorage.getItem("id");
            sources.forEach(item=> {
                $.ajax({
                    type: "post",
                    url: "/Chrono/deletPhoto/",
                    data: { path: item},
                    success: function () {
                        $(".share-area").empty();
                    },
                });
            });
        }
    }
    $(document).on('click', '#share-all', function (){
        let text = new String($(".area").val());
        sources.forEach(item=> {
            $.ajax({
                type: "post",
                url: "/Chrono/ShareNewPost/",
                data: {src:item ,text: text },
                success: function () {
                    $(".area").val("");
                    $(".picture-area").empty();
                    $(".picture-area").append(`<label class="text-success">successfuly done</label>`);
                    sources.length = 0;
                    location.reload();
                },
            });
        });
    });
});