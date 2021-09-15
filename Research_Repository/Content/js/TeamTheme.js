//Edits name of teams and themes
function editName(thisbtn, type) {
    event.preventDefault();
    //Show
    $(thisbtn).siblings(`.${type}-name-input`).show();
    $(thisbtn).siblings(`.apply-${type}-name-btn`).show();

    //Hide
    $(thisbtn).siblings(`.${type}-name-text`).hide();
    $(thisbtn).hide();
    console.log(`.${type}-name-input`);
};

function applyName(thisbtn, type) {
    event.preventDefault();
    //Change text value
    let newTeamName = $(thisbtn).siblings(`.${type}-name-input`).val();
    $(thisbtn).siblings(`.${type}-name-text`).html(newTeamName);

    //Show
    $(thisbtn).siblings(`.${type}-name-text`).show();
    $(thisbtn).siblings(`.edit-${type}-name-btn`).show();

    //Hide
    $(thisbtn).siblings(`.${type}-name-input`).hide();
    $(thisbtn).hide();
    console.log(`.${type}-name-input`);
};