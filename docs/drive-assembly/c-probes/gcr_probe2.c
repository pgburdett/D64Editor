/* Second attempt at raw burst track reads with bus setup (TALK / LISTEN / B-R:0).
   All variants still fail on the XUM1541: no raw GCR support without a parallel cable. */
/* Build: gcc -O0 -g gcr_probe2.c -lopencbm -o gcr_probe2                       */
#include <opencbm.h>
#include <stdio.h>
#include <string.h>

static void try(int mode, CBM_FILE f, unsigned char *buf)
{
    int rc;
    memset(buf, 0, 8192);
    if (mode == 1) cbm_talk(f, 8, 0);
    if (mode == 2) { cbm_listen(f, 8, 15); cbm_unlisten(f); }
    if (mode == 3) cbm_exec_command(f, 8, "B-R:0", 5);
    rc = cbm_srq_burst_read_track(f, buf, 8192);
    printf("srq mode=%d rc=%d first8=", mode, rc);
    for (int i = 0; i < 8; i++) printf("%02X ", buf[i]);
    printf("\n");
    cbm_untalk(f);
}

int main(void)
{
    CBM_FILE f;
    unsigned char buf[8192];
    if (cbm_driver_open(&f, 0)) { printf("driver_open failed\n"); return 1; }
    for (int m = 0; m <= 3; m++) try(m, f, buf);
    cbm_driver_close(f);
    return 0;
}
