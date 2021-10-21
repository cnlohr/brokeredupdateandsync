#include <stdio.h>

#define MINIZ_IMPLEMENTATION
#include "miniz.h"

#define STBI_NO_SIMD
#define STB_IMAGE_IMPLEMENTATION
#include "stb_image.h"

#define STB_IMAGE_WRITE_IMPLEMENTATION
#include "stb_image_write.h"
	
#define WIDTH  256
#define HEIGHT 256

#include "unitytexturewriter.h"


uint32_t framebuffer[WIDTH][HEIGHT];
uint32_t framebuffer_out[256][256];

int main()
{
	mz_zip_archive zip_archive = { 0 };
	mz_bool status;
	int i;

	status = mz_zip_reader_init_file(&zip_archive, "CC0_Textures.zip", 0);
	if (!status)
	{
		printf("mz_zip_reader_init_file() failed!\n");
		return EXIT_FAILURE;
	}
	
	int blockx = 0, blocky = 0;
	
	int files = (int)mz_zip_reader_get_num_files(&zip_archive);

	printf( "Opened. Zips: %d\n", files );

	for (i = 0; i < files; i++)
	{
		mz_zip_archive_file_stat file_stat;
		if (!mz_zip_reader_file_stat(&zip_archive, i, &file_stat))
		{
			printf("mz_zip_reader_file_stat() failed!\n");
			mz_zip_reader_end(&zip_archive);
			return EXIT_FAILURE;
		}

		// See if it's a blocks/*.png
		const char * fn = file_stat.m_filename;
		if( strncmp( fn, "blocks/", 7 ) != 0 || strncmp( fn+strlen(fn)-4, ".png", 4 ) != 0 )
			continue;

		size_t uncomp_size;
		uint8_t * p = mz_zip_reader_extract_file_to_heap( &zip_archive, fn, &uncomp_size, 0 );
		if( !p )
		{
			fprintf( stderr, "mz_zip_reader_extract_file_to_heap() failed on %s\n", fn );
			mz_zip_reader_end(&zip_archive);
			return EXIT_FAILURE;
		}
		
		
		int w, h, channels;
		uint32_t * pixels = (uint32_t*)stbi_load_from_memory( p, uncomp_size, &w, &h, &channels, 4 );
		if( !pixels )
		{
			fprintf( stderr, "Error: Failed to decode %s\n", fn );
			return EXIT_FAILURE;
		}
		printf( "Found: %s / %d bytes [%d %d %d]\n", fn, uncomp_size, w, h, channels );
		if( w != 16 || h != 16 )
		{
			fprintf( stderr, "Warning: block wrong size.\n" );
			continue;
		}
		
		//Copy block in.
		int x, y;
		for( y = 0; y < h; y++ )
			for( x = 0; x < w; x++ )
			{
				framebuffer[blocky+y][blockx+x] = pixels[y*w+x];
			}
		
		//Advance block pointer.
		blockx = blockx+w;
		if( blockx >= WIDTH )
		{
			blockx = 0;
			blocky+=h;
			if( blocky >= HEIGHT )
			{
				fprintf( stderr, "Warning: image full.\n" );
				break;
			}
		}
		free( pixels );
		mz_free(p);
	}
	mz_zip_reader_end(&zip_archive);
	
	int w, h, n;
	uint32_t * pixels;
	
	int eb = 0;
	do
	{
		//Advance block pointer.
		blockx = blockx+w;
		if( blockx >= WIDTH )
		{
			blockx = 0;
			blocky+=h;
			if( blocky >= HEIGHT )
			{
				fprintf( stderr, "Warning: image full.\n" );
				break;
			}
		}
		char fnn[1024];
		sprintf( fnn, "ExtraBlocks/%d.png", eb );
		printf( "Opening %s\n", fnn );
		pixels = (uint32_t*)stbi_load( fnn, &w, &h, &n, 4);
		if( !pixels ) break;
		if( n != 4 )
		{
			fprintf( stderr, "Error: Can't parse SnapIcon.png.\n" );
			return EXIT_FAILURE;
		}
		int x, y;
		for( y = 0; y < h; y++ )
			for( x = 0; x < w; x++ )
			{
				framebuffer[blocky+y][blockx+x] = pixels[y*w+x];
			}
		free( pixels );

		eb++;
	} while( 1 );


	pixels = (uint32_t*)stbi_load( "SnapIcon.png", &w, &h, &n, 4);
	if( n != 4 )
	{
		fprintf( stderr, "Error: Can't parse SnapIcon.png.\n" );
		return EXIT_FAILURE;
	}
	int x, y;
	for( y = 0; y < h; y++ )
		for( x = 0; x < w; x++ )
		{
			framebuffer[(15*16)+y][(13*16)+x] = pixels[y*w+x];
		}
	free( pixels );

	pixels = (uint32_t*)stbi_load( "GravityIcon.png", &w, &h, &n, 4);
	if( n != 4 )
	{
		fprintf( stderr, "Error: Can't parse GravityIcon.png.\n" );
		return EXIT_FAILURE;
	}
	for( y = 0; y < h; y++ )
		for( x = 0; x < w; x++ )
		{
			framebuffer[(15*16)+y][(14*16)+x] = pixels[y*w+x];
		}
	free( pixels ); 

	pixels = (uint32_t*)stbi_load( "MotionIcon.png", &w, &h, &n, 4);
	if( n != 4 )
	{
		fprintf( stderr, "Error: Can't parse MotionIcon.png.\n" );
		return EXIT_FAILURE;
	}
	for( y = 0; y < h; y++ )
		for( x = 0; x < w; x++ )
		{
			framebuffer[(15*16)+y][(12*16)+x] = pixels[y*w+x];
		}
	free( pixels ); 

	pixels = (uint32_t*)stbi_load( "SimpleIcon.png", &w, &h, &n, 4);
	if( n != 4 )
	{
		fprintf( stderr, "Error: Can't parse SimpleIcon.png.\n" );
		return EXIT_FAILURE;
	}
	for( y = 0; y < h; y++ )
		for( x = 0; x < w; x++ )
		{
			framebuffer[(15*16)+y][(11*16)+x] = pixels[y*w+x];
		}
	free( pixels ); 


	int ret = stbi_write_png( "block_atlas.png", WIDTH, HEIGHT, 4, framebuffer, 4*WIDTH );
	printf( "stbi_write_png = %d\n", ret );

	for( i = 0; i < 256; i++ )
	for( y = 0; y < 16; y++ )
	for( x = 0; x < 16; x++ )
	{
		framebuffer_out[i][x+y*16] = framebuffer[(i%16)*16+x][(i/16)*16+y];
	}

	WriteUnityImageAsset( "block_atlas_array.asset", framebuffer_out, sizeof(framebuffer_out), 16, 16, 256, UTE_RGBA32 | UTE_FLAG_TEXTUREARRAY );

	return 0;
}
