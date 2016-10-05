function adicionaItem(elemento, item) {
    var width = (item.width == undefined) ? 100 : item.width;
    var height = (item.height == undefined) ? 100 : item.height;
    var title = (item.title == undefined) ? "undefined" : item.title;
    var src = (item.src == undefined) ? "" : item.src;
    var href = (item.href == undefined) ? "#" : item.href;
    var alvo = "<a href='" + href + "' class='work-item' style='width : " + (width + 30) + "px; height: " + (height + 30 + 30) + "px; align='center'>" +
                        "<img width='" + width + "' height='" + height + "' src='" + src + "'>" +
                        "<span>" + title + "</span>" +
                   "</a>";
    $(alvo).appendTo(elemento);

}