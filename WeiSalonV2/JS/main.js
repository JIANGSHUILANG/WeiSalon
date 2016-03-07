$(function () {
    if ($('#show').length) {
        GetHtml(val);
    }
    if ($('.focus').length) {
        $('.focus').focuschange({ auto: false, cycle: true });
    }

    var iWinW = $('body').width();

    //$(window).on("orientationchange", function () {
    //    iWinW = $('body').width();
    //    //设计师瀑布流
    //    //if ($('#design').length) {
    //    //    loadPubuliu($('#design'), 1);
    //    //}
    //    ////作品展示瀑布流
    //    //if ($('#show').length) {
    //    //    loadPubuliu($('#show'), 2);
    //    //}
    //});

    $('.open-menu').on('click', function () {
        var iStopW = iWinW / 3 > 300 ? iWinW / 3 : 300;
        $('.sidebar').show().animate({ 'opacity': 1 }, 500, function () {
        }).find('.inside').width(parseInt(iStopW));
    });

    $('.sidebar').on({
        'swipeLeft': function (e) {
            $(this).animate({ 'opacity': 0.5 }, 500, function () {
                $(this).hide();
            }).find('.inside').width(0);
        },
        'click': function (e) {
            if (e.target.className == 'sidebar') {
                $(this).animate({ 'opacity': 0.5 }, 500, function () {
                    $(this).hide();
                }).find('.inside').width(0);
            }
        }
    });
    pageind = 1;
    window.addEventListener('load', function () {
        //设计师瀑布流
        if ($('#design').length) {
            var state = true;
            $(window).on('scroll', function () {
                if (!state) return false;
                if ($(document).height() - $(window).height() - $('body').scrollTop() < 10) {
                    var _self = $('#design');
                    state = false;
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: '/Salon/hairList',
                        data: { uid: $('body').data('huid'), pgsize: 10, ind: ++pageind },
                        success: function (data) {
                            var appendDom = '';
                            if (data.status == 1) {
                                for (var i = 0; i < data.info.length; i++) {
                                    appendDom += "<dl style=\"margin-bottom: 20px;\">"
                                    + "<dt><a class=\"pic\" data-uid=\"" + data.info[i].Huid +
                                    "\" data-zan=\"" + 11 +
                                    "\" href=\"javascript:;\"><img src=\"" + data.info[i].Logo +
                                    "?imageView2/1/w/150/h/200\" width=\"150px\" style=\"height:\"200px\"></a></dt>"
                                    + "<dd><span class=\"name\">" + data.info[i].Name +
                                    "</span><span class=\"goodat\">擅长：" + data.info[i].Exp + "</span><span class=\"price\">价格：" +
                                    data.info[i].Hairprice + "元</span></dd>"
                                    + "</dl>";
                                }
                                state = true;
                            }
                            _self.append(appendDom);
                        }
                    });
                }
            });
        }
    }, false);

    $('#show').on('click', '.pic', function () {
        $('.detail').height($(document).height()).show();
        $('body').scrollTop(0);
        var iOnceW = $('.show-detail').width();

        $('.show-detail .show-detail-big').attr('src', $(this).find('img').attr('src'));
        $('.show-detail .design-info .design-pic img').attr('src', $(this).parents('dl').find('.design-pic img').attr('src'));
        $('.show-detail .design-info .info').html($(this).parents('dl').find('.info').html());
    });

    $('#design').on('click', '.pic', function () {

        $('.detail').height($(document).height()).show();
        $('body').scrollTop(0);
        var iOnceW = $('.designer-works').width();
        var _this = $(this);
        $('.designer-photo img').attr('src', _this.find('img').attr('src'));
        $('.designer-info .name').html(_this.parents('dl').find('.name').html());
        $('.designer-info .goodat').html(_this.parents('dl').find('.goodat').html());
        $('.designer-info .jiage').html(_this.parents('dl').find('.price').html());
        $('.designer-info .zan span').html(_this.data('zan'));
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: '/Salon/showList2',
            data: { uid: _this.data('uid'), },
            success: function (data) {
                var imglist = ''
                if (data.status != 0) {
                    for (var i = 0; i < data.info.length; i++) {
                        imglist += '<li><img src="' + imgpath + data.info[i].bgimage + '"></li>';
                    };
                    $('.designer-works .pic').width(data.length * iOnceW / 3).html('').append(imglist);
                    $('.designer-works .fulcrum').attr('src', imgpath + data.info[0].bgimage).width(iOnceW / 3 * 0.9).show();
                    $('.designer-works .pic li').width(iOnceW / 3);
                    $('.designer-works .idx').show();
                    $('.designer-works').focuschange({ auto: false, cycle: false, once: 3 });
                }
            }
        });

    });
    $('.back').click(function () {
        $('.detail').hide();
    });


    //瀑布流展示
    var $masonry = $('#show').length ? $('#show') : $('#design');

    if ($masonry.length) {

        $masonry.imagesLoaded(function () {
            $masonry.masonry({
                itemSelector: 'dl',
                gutter: 0,
                isAnimated: true
            });
        });

        var ajaxState = true;
        var oNextData = {
            uid: $('body').data('huid'),
            GType: $('body').data('type')
        };

        //setTimeout(function () {
        //    var $boxes = '<dl><dt><a class="pic" href="javascript:;"><img src="http://g.hiphotos.baidu.com/image/pic/item/4a36acaf2edda3ccd2994a5402e93901213f9241.jpg"></a></dt></dl><dl><dt><a class="pic" href="javascript:;"><img src="http://a.hiphotos.baidu.com/image/pic/item/2fdda3cc7cd98d10b1a9fe45223fb80e7bec9041.jpg"></a></dt></dl><dl><dt><a class="pic" href="javascript:;"><img src="http://b.hiphotos.baidu.com/image/pic/item/14ce36d3d539b600ab94fdc3ea50352ac65cb768.jpg"></a></dt></dl>';
        //    $boxes = $($boxes);
        //    $boxes.imagesLoaded(function () {
        //        $masonry.append($boxes).masonry('appended', $boxes);
        //    });
        //}, 3000)

        $masonry.data('pageindex', 1);
        $(window).on('scroll', function () {
            if (!ajaxState) return false;

            if ($(document).height() - $(window).height() - $('body').scrollTop() < 10) {

                $masonry.data('pageindex', parseInt($masonry.data('pageindex')) + 1);
                oNextData.ind = parseInt($masonry.data('pageindex'));

                ajaxState = false;
                var appendDom = "";
                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    url: '/Salon/fenye',
                    data: oNextData
                }).done(function (data) {
                    if (data.status == "1") {
                        for (var i = 0; i < data.info.length; i++) {
                            if ($masonry[0].id === 'design') {
                                appendDom += '<dl>'
                                    + '<dt><a class="pic" data-uid="' + data.info[i].Huid + '" data-zan="' +
                                   11 + '" href="javascript:;"><img src="' +
                                     data.info[i].Logo + '"></a></dt>'
                                    + '<dd><span class="name">' + data.info[i].Name +
                                    '</span><span class="goodat">擅长：' + data.info[i].Exp +
                                    '</span><span class="price">价格：' + data.info[i].Hairprice + '元</span></dd>'
                                    + '</dl>';
                            }
                            else if ($masonry[0].id === 'show') {
                                appendDom += '<dl>'
                                    + '<dt><a class="pic" data-uid="' + data.info[i].bgid +
                                    '" href="javascript:;"><img src="' + imgpath + data.info[i].bgimage +
                                    '"></a></dt>'
                                    + '<dd><div class="design-info"><a class="design-pic" href="javascript:;" data-uid="' +
                                    data.info[i].bguid + '"><img src="' + imgpath + data.info[i].logo +
                                    '"></a><span class="info">' + data.info[i].bgname +// '<br>擅长：' + data[i]["goodat"] +
                                    '</span></div></dd>'
                                    + '</dl>';
                            }
                        }
                        //$newDom = $(appendDom);
                        //$masonry.append($newDom).masonry('appended', $newDom);
                        //$masonry.imagesLoaded(function () {
                        //    $masonry.masonry('layout');
                        //});
                        $newDom = $(appendDom);
                        $newDom.css('opacity', 0).appendTo($masonry);

                        console.time("test");
                        $newDom.imagesLoaded(function () {
                            $newDom.css('opacity', 1);
                            console.timeEnd("test");
                            $masonry.masonry('appended', $newDom);
                            //$masonry.masonry('appended', $newDom);
                        });
                        ajaxState = true;
                    }
                });
            }
        });
    }

});

