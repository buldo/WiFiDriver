namespace WiFiDriver.App.Rtl8812au;

public static class rtw_cmd
{
    public static bool rtw_setopmode_cmd(_adapter adapter, RTW_CMDF flags)
    {
        //cmd_obj cmdobj;
        setopmode_parm parm = new setopmode_parm()
        {
            mode = NDIS_802_11_NETWORK_INFRASTRUCTURE.Ndis802_11Monitor
        };
        //cmd_priv pcmdpriv = adapter.cmdpriv;
        //submit_ctx sctx;
        bool res = true;

        /* prepare cmd parameter */


        //if (flags.HasFlag(RTW_CMDF.RTW_CMDF_DIRECTLY))
        {
            /* no need to enqueue, do the cmd hdl directly and free cmd parameter */
            if (true != setopmode_hdl(adapter, parm))
            {
                return false;
            }
        }
        //else
        //{
        //    throw new NotImplementedException();
        //    ///* need enqueue, prepare cmd_obj and enqueue */
        //    //cmdobj = (cmd_obj *)rtw_zmalloc(sizeof(*cmdobj));
        //    //if (cmdobj == NULL)
        //    //{
        //    //    res = _FAIL;
        //    //    rtw_mfree((u8*)parm, sizeof(*parm));
        //    //    goto exit;
        //    //}

        //    //init_h2fwcmd_w_parm_no_rsp(cmdobj, parm, _SetOpMode_CMD_);

        //    //if (flags & RTW_CMDF_WAIT_ACK)
        //    //{
        //    //    cmdobj.sctx = &sctx;
        //    //    rtw_sctx_init(&sctx, 2000);
        //    //}

        //    //res = rtw_enqueue_cmd(pcmdpriv, cmdobj);

        //    //if (res == _SUCCESS && (flags & RTW_CMDF_WAIT_ACK))
        //    //{
        //    //    rtw_sctx_wait(&sctx, __func__);
        //    //    _enter_critical_mutex(&pcmdpriv.sctx_mutex, NULL);
        //    //    if (sctx.status == RTW_SCTX_SUBMITTED)
        //    //        cmdobj.sctx = NULL;
        //    //    _exit_critical_mutex(&pcmdpriv.sctx_mutex, NULL);
        //    //}
        //}

        exit:
        return res;
    }
}