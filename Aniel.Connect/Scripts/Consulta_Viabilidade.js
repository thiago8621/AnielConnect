/**
 * Ato de verificar a disponibilidade em um endereço
 * @param {any} objeto Objeto principal com parametros necessários @param {any} parametros parametros necessários da consulta */
async function verificar_disponibilidade(objeto, parametros) {

    //let problem = `Problema Interno com Campo ${objeto.tipo}`;

    if (parametros.hasOwnProperty("map")) { initMap(); }

    let matrizLat = Number($(`#iLatitude${objeto.tipo}`).val().replace(',', '.')),
        matrizLng = Number($(`#iLongitude${objeto.tipo}`).val().replace(',', '.'));
    console.log(objeto.id);


    if (!((matrizLat == 0 || matrizLng == 0))) {

        let response_Consulta = await getConsultaViabilidade(
            {
            latitude: matrizLat, longitude: matrizLng, IdVenda: objeto.venda,
            IdCombo: objeto.combo, IdCampanha: objeto.campanha
            }, parametros);
        //lista_Analise_Comercial.push(response_Consulta);

        if (response_Consulta.mensagem !== null) {
            //Erro 
            Notificar(`${parametros["Atencao"]}`, response_Consulta.mensagem, "warning");
            if (objeto.tipo !== parametros["Prospect"]) {

                $(`#CliqueAqui${objeto.tipo}`).attr("hidden", true);
                
            }
        }

        //if (response_Consulta.objeto?.inviabilidade !== null) {            
        //    //Sem Disponibilidade
        //    let Id_Inviabilidade = response_Consulta.objeto.inviabilidade.id;
        //    Notificar(`${parametros["Atencao"]}`, response_Consulta.objeto.inviabilidade.motivo, "warning");

        //    if (objeto.tipo == parametros["Prospect"]) {

        //        $("#iStatusProspecto").val("3").trigger(`change`);
        //        AddSelect("iMotivoInviabilidade", parametros["InviabilidadeCombo"], parametros["Paginacao"], null, null, `${Id_Inviabilidade}`, parametros["Selecione"], efetivadobool);

        //        $(`#CliqueAqui${objeto.tipo}`).attr("hidden", true);

        //    }

        //    else {

        //       // $("#iStatusProspecto").val("3").trigger(`change`);

        //    }

        //    return false;
        //}

        if (response_Consulta.objeto !== null) {
            let caixas = response_Consulta.objeto;

            $("#iCaixa_Ftth").val(caixas[0].id);
            $("#iPorta_Ftth").val(caixas[0].portasDisponiveis[0]);

            Notificar(`${parametros["Informacao"]}`, "Cliente possui disponibilidade neste endereço", "info");
            $(`#CliqueAqui${objeto.tipo}`).attr("hidden", false);

            return true;
        }

        //else {

        //    Notificar(`${parametros["Atencao"]}`, problem, "warning");
        //}

        return false;
    }

    else {

        Notificar(`${parametros["Atencao"]}`, " O Endereço é obrigatório. Por Favor Preencher!", "warning");

        $(`#CliqueAqui${objeto.tipo}`).attr("hidden", true);

        return false;
    }
}

/** 
 * @param {any} obj Objeto principal
 * @param {any} parametros
 */
async function getConsultaViabilidade(obj, parametros) {

    let geoCliente = [],
        lista_Analise_Comercial = [],
        response = null;
    try {
        geoCliente.push(obj.latitude, obj.longitude);

        await $.ajax({
            type: "GET",
            url: `${parametros["urlPortal"]}?latitude=${obj.latitude}&&longitude=${obj.longitude}&&IdVenda=${obj.IdVenda}`,
            //data: { Latitude: obj.latitude, Longitude: obj.longitude, IdVenda: obj.IdVenda, IdCombo: obj.IdCombo, IdCampanha: obj.IdCampanha, },
            dataType: "json"
        })
            .done(function (data) {

                response = data;
            });
    }
    catch (exception) {
        console.log(exception);
    }
    return response;
}

function Visualizar_Caixa(lista_Analise_Comercial, geoCliente, parametros) {

    const
        formulario = document.createElement("form"),
        formdata = document.createElement("input"),
        formgeo = document.createElement("input"),
        formPoligonoBanco = document.createElement("input");

    //Formulário
    formulario.method = "POST";
    $(formulario).attr({
        action: `${parametros["indexViabilidade"]}`,
        target: "_blank"
    });

    //Data do Formulário  
    $(formdata).attr({
        type: "hidden",
        name: "ObjJson",
        value: JSON.stringify(lista_Analise_Comercial)
    }).appendTo(formulario);

    //Dados Geograficos    
    $(formgeo).attr({
        type: "hidden",
        name: "GeoCliente",
        value: JSON.stringify(geoCliente)
    }).appendTo(formulario);

    //Dados de Polígonos do Banco
    $(formPoligonoBanco).attr({
        type: "hidden",
        name: "PoligonoBanco",
        value: JSON.stringify(poligonosbanco)
    }).appendTo(formulario);

    $(document.body).append(formulario);
    formulario.submit();
}
