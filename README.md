# CDS_Mapper
CDS Mapper is a tool to map specific positions, such as methylation sites or motif positions, on bacterial genomes to genes described in their corresponding GenBank files.
CDS mapper can also map the query positions to specific length upstream and downstream of the genes annotated in the GenBank file.

This application has been specifically written for handling bacterial datasets and has not been tested for eukaryotes. It has a simple and intuitive GUI for ease of use.

Tested with the following speficiations: (Note - This is not indicative of the minimum specifications required to run the application)

Hardware:
1) CPU - AMD Ryzen 5 2600 (3.4 GHz, 6 Cores - 12 Threads), 11th Gen Intel Core i7-1165G7 (2.8 GHz, 4 Cores, 8 Logical Processors)
2) RAM - Patriot Viper (16 GB, 3000 MHz), 16GB LPDDR4X
3) GPU - Nvidia RTX 2070 (8 GB), Intel Iris Xᵉ Graphics

Software:
1) Operating System - Windows 10 64-bit (Build - 19044.1415), Windows 11 64-bit
2) IDE - Visual Studio 2019 (Version - 16.7.6)
3) .NET Farmework (Version - 4.7.2)

Installation: 
1) Download the latest release from Releases under Assets (most recent release appears first).
2) Unzip the folder and run "Setup.msi".
3) Select directory for installation along with other preferred settings and click Next.
4) Once the setup has finished installing, click Finish.

Required input files:
1) List of positions to be queried present on the +strand of the reference genome in BED format (format described in http://genome.ucsc.edu/FAQ/FAQformat#format1)
2) List of positions to be queried present on the -ve or complementary strand of reference genome in BED format
3) GenBank file with annotated genome (File format: https://www.ncbi.nlm.nih.gov/Sitemap/samplerecord.html)

Output file formats:

The outputs generated using the application can be exported either as a tab delimited text(.txt) file or as comma-separated values(.CSV). 
