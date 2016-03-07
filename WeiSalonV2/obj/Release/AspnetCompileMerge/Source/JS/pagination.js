function page(type) {
    if (type == 0) {
        if (index == count) {
            index = count;
        } else {
            index++;
        }
        location.href = url.replace("(p)", index);
    }
    else {
        if (index == 1) {
            index = 1;
        } else {
            index--;
        }
        location.href = url.replace("(p)", index);
    }
}


function pageinit() {
    if (index == 1) {
        $(".paging a").eq(0).addClass("disabled");
    }
    else if (index == count) {
        $(".paging a").eq(1).addClass("disabled");
    } else {
        $(".paging a").eq(0).addClass("");
        $(".paging a").eq(1).addClass("");
    }
}





