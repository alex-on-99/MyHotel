function RoomsFiltration() {
    let currentClass = $("#class-selector").val();
    let currentCount = $("#count-selector").val();
    let blocks = $(".free-rooms-block");

    for (var i = 0; i < blocks.length; i++) {
        if (currentClass == "noOption" && currentCount == "noOption") {
            $(blocks[i]).css("display", "flex")
        } else {
            let classText = $(blocks[i]).find(".room-class-value");
            let roomClass = $(classText[0]).text().replace(/\s/g, "");
            let countText = $(blocks[i]).find(".room-count-value");
            let roomCount = $(countText[0]).text().replace(/\s/g, "");

            if (currentClass != "noOption" && currentCount == "noOption") {
                if (roomClass != currentClass) {
                    $(blocks[i]).css("display", "none");
                } else if (roomClass == currentClass && currentCount == "noOption") {
                    $(blocks[i]).css("display", "flex");
                }
            }

            if (currentClass == "noOption" && currentCount != "noOption") {
                if (roomCount != currentCount) {
                    $(blocks[i]).css("display", "none");
                } else if (roomCount == currentCount && currentClass == "noOption") {
                    $(blocks[i]).css("display", "flex");
                }
            }

            if (currentClass != "noOption" && currentCount != "noOption") {
                if (roomCount != currentCount || roomClass != currentClass) {
                    $(blocks[i]).css("display", "none");
                } else{
                    $(blocks[i]).css("display", "flex");
                }
            }
        }
    }

}

function RoomSort() {
    let currentSort = $("#sort-selector").val();
    let blocks = $(".free-rooms-block");

    for (let i = 0; i < blocks.length; i++) {
        for (let j = 0; j < blocks.length - 1; j++) {
            let firstRoomPriceText = $(blocks[j]).find(".room-price-value");
            let firstRoomPrice = $(firstRoomPriceText[0]).text().replace(/\s/g, "");
            let secondRoomPriceText = $(blocks[j+1]).find(".room-price-value");
            let secondRoomPrice = $(secondRoomPriceText[0]).text().replace(/\s/g, "");

            if (currentSort == "up") {
                if (parseInt(firstRoomPrice) > parseInt(secondRoomPrice)) {
                    let temp = blocks[j];
                    blocks[j] = blocks[j + 1];
                    blocks[j + 1] = temp;
                }
            } else {
                if (parseInt(firstRoomPrice) < parseInt(secondRoomPrice)) {
                    let temp = blocks[j];
                    blocks[j] = blocks[j + 1];
                    blocks[j + 1] = temp;
                }
            }
        }
    }

    $("#free-rooms-block-list").empty();

    for (let i = 0; i < blocks.length; i++) {
        $("#free-rooms-block-list").append(blocks[i]);
    }
}

$("#class-selector").on("change", RoomsFiltration);
$("#count-selector").on("change", RoomsFiltration);
$("#sort-selector").on("change", RoomSort);