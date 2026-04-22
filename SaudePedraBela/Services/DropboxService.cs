using Dropbox.Api;
using Microsoft.Extensions.Options;
using SaudePedraBela.Models;
using System;
using System.IO;
using System.Threading.Tasks;


namespace SaudePedraBela.Services
{
    public class DropboxService
    {
        private readonly DropboxSettings _settings;

        // O construtor agora recebe as configurações via Injeção de Dependência
        public DropboxService(IOptions<DropboxSettings> settings)
        {
            _settings = settings.Value;
        }

        private DropboxClient GetClient()
        {
            // O SDK cuida da renovação usando o RefreshToken quando o AccessToken expira
            return new DropboxClient(
                _settings.RefreshToken,
                _settings.AppKey,
                _settings.AppSecret);
        }

        public async Task<string> UploadArquivo(string pasta, string nomeArquivo, Stream conteudo)
        {
            using (var dbx = GetClient())
            {
                var caminho = $"{pasta}/{nomeArquivo}".Replace("//", "/");
                await dbx.Files.UploadAsync(caminho, Dropbox.Api.Files.WriteMode.Overwrite.Instance, body: conteudo);

                var sharedLink = await dbx.Sharing.CreateSharedLinkWithSettingsAsync(caminho);
                return sharedLink.Url.Replace("dl=0", "raw=1");
            }
        }

        public async Task DeletarArquivo(string caminhoArquivo)
        {
            using (var dbx = GetClient())
            {
                try
                {
                    await dbx.Files.DeleteV2Async(caminhoArquivo);
                }
                catch (Exception ex)
                {
                    // Trate erros como "arquivo não encontrado" aqui
                    throw new Exception($"Erro ao deletar arquivo: {ex.Message}");
                }
            }
        }
    }
}

//antes de enviar o push alterações:
/*
 * - Alterado o appsettings.json para incluir as chaves de acesso ao DropBox
 * - Alterado o Program.cs para habilitar a injeção de dependência do DropboxService e DropboxSettings
 * - Criado arquivo DropboxSettings.cs para armazenar as chaves de acesso ao DropBox
 * - Criado o código do DropboxService.cs para realizar as operações de upload e delete de arquivos no DropBox
 */

//Codigos para configurar token de acesso ao DropBox
/*
 CHAVES DE ACESSO
-----------------------------------------------
App key
j4t8aw63o3u1kzc

App secret
2n9yo63r8zjeu72
-----------------------------------------------

TOKEN DE ATUALIZAÇÃO: (Utilizado para gerar tokens de acesso)
-----------------------------------------------
"refresh_token": "qvJihQlNpvIAAAAAAAAAAWH2MkCUHD0DLuHuKXXVGyyuSFeZq5ZPfYT27FIh0ydp",
-----------------------------------------------


COMANDO PARA GERAR UM NOVO TOKEN DE ACESSO (VALIDADE 4Hrs):
-----------------------------------------------
set APP_KEY=j4t8aw63o3u1kzc
set APP_SECRET=2n9yo63r8zjeu72
set REFRESH_TOKEN=qvJihQlNpvIAAAAAAAAAAWH2MkCUHD0DLuHuKXXVGyyuSFeZq5ZPfYT27FIh0ydp
curl -X POST https://api.dropbox.com/oauth2/token ^
-d refresh_token=%REFRESH_TOKEN% ^
-d grant_type=refresh_token ^
-d client_id=%APP_KEY% ^
-d client_secret=%APP_SECRET%
-----------------------------------------------


EXEMPLO DE TOKEN DE ACESSO:
-----------------------------------------------
{"access_token": "sl.u.AGYxjv3O3ShE1wZSiNT5WTt6l8DEPal7cUYWYIqff2U0S9cXWRX-5cdD7wCIBZCb5zOP1EQ9X-P_12HtWqpdS_xOMbqOYknvT_H1623QpDsXyTBW1CZe9-EDfJaZl6AT_MvbQhxQ43--jA4mtGJhGqS0TKXoW14OIDyQcLZ7XAU6HTQAFZfaD_KMQGAF5m1A1CQsYVsTLw9JpzrJVYIJ-5MwJLJcKFtfRPEPsTU4PvjHfar6sTGyZII4TtgFbBhf7yRIf5yaEhwmU-PNJDTEiq1YapAX3kLfWjEAhwQ5bcAUoKIJqXFK0sc_6ifIdEeiL9zsAC-IViitufTmpTAQ1apuhwwUNEWZ8Ldz5JWirs7d30Fmz6vMqjZavFOUwYw-lcfpmrHvIuUpdq8saB5o3wMlT3kt8bE4RnO7gcESxIFdf2vZYljorkuI3iC3M8n6zDALtbjCrWICdcnI_p-7hHmd5349E7knmul6JXoTpRydihpvRXdufZbAbY5HGAo9u-cXa60vnGgX10EFZcm9MyGjCV8ZdXxx49NEr4gR2Yny8L8v2AfegYbVEoZiyhHurEpd46rQprBCpeZvGBTvJw9cNrg_nxFrxEapbAlrNX-j9GuKp_IfAfxoBjK2RsUU07UOqFlQfptwl3LSd73AyNO_7FCT-BUsPtOVAvgQZHNQjcyGljY0QZT_nrUgeVht2cRGajn1oZH1OmJWe2fr5ssPfNQ5X8hnRyJajuMYjjbh1Yrw8OakJH9tYQKSDeiuRqBKY-oZNSJrH-VcnokwCJf4Smk2k4SFTTz4NA_xiQv4MGj6UIPQOGtTVwu_2ofgGlrzxxfeST9lNf4upvf4irJ1vx3YuJoEPm9CSLCdACL7Q1NXnNbcMlfgL0uYOL7v2_yDXdIM39JxvCFs5JQLHE2zIGrxLgfIahjhT2zjVxSbcrBzaTvsFD6ptnEzIjyVEYUha8EbPa2BT42RwTna8FyVsdQc0faKRur7SbTNK6vHOgnjpL4OxejE3zV3czBNiGqDPQofg0SihSIn1Px3KvaIsLeF4bEadVVuGNK67KKSH6PX59U386dhl1LppkcUsHFBTe0Sw1Intsz0omvxjlxjA6zK7zxbEebabaLuKw0kH_c_uo0AdIxcM7y53a-yDm9siL4W8-KrZQ35Rx3FJcnwsq--BUpDkoO5LfBTfx9-9fBfC5DLtcWGRp6P24XnyHUiw09di4wr-gt84xQncqp9HmohR9gR_wajI9ahoND8czVrRmhyuZcUQGTe8rCDS7K5T9sD0HG7RhpIvI0clrOuEqZdiV79DMHzKdo-CHU1FRyYVGIj8wk8e41VEo2-pBo-AWeJBlxqk8rNEg9BQZUk-3-mZDevWpee_rDKAdYBAIR5wnwVm8tVb_AycSODa905U7C6T_jJl4H6Z4arhuVkg_xznKP_Gf-wFRd4Q7bO1BEyNDKR8xxNhKblB9kQzIntSu_pJbDhzAEIHd2bYM0E", "token_type": "bearer", "expires_in": 14400}
-----------------------------------------------


 */