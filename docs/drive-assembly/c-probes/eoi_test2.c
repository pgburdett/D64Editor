/* Reproduce VICE 3.10's failing call sequence: open with empty name, send the
   filename as raw data afterwards, then talk/read. Returns n=0, eoi=1 immediately. */
/* Build: gcc -O0 -g eoi_test2.c -lopencbm -o eoi_test2                        */
#include <opencbm.h>
#include <stdio.h>
#include <string.h>

int main(void)
{
    CBM_FILE f;
    if (cbm_driver_open(&f, 0)) { printf("driver_open failed\n"); return 1; }

    printf("cbm_open(dev=8, sec=0, name=\"\", len=0)\n");
    printf("open=%d\n", cbm_open(f, 8, 0, "", 0));
    printf("raw_write('*')=%d\n", cbm_raw_write(f, "*", 1));
    cbm_unlisten(f);
    printf("talk=%d\n", cbm_talk(f, 8, 0));

    for (int i = 0; i < 200; i++)
    {
        unsigned char c = 0;
        int n = cbm_raw_read(f, &c, 1);
        int eoi = cbm_get_eoi(f);
        if (i < 8 || eoi || n <= 0)
            printf("%04d n=%d eoi=%d byte=%02X\n", i, n, eoi, c);
        if (eoi || n <= 0) break;
    }

    cbm_untalk(f);
    cbm_close(f, 8, 0);
    cbm_driver_close(f);
    return 0;
}
